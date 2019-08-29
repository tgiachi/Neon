using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Data.Mqtt;
using Neon.Api.Interfaces.Services;
using System;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("MQTT Service", "Connect to MQTT queue", LoadOrder = 2)]
	public class MqttService : IMqttService
	{
		private readonly ILogger _logger;
		private readonly MqttConfig _config;
		private IMqttClient _mqttClient = null;
		private IMqttServer _mqttServer = null;
		private int _clientConnectionRetryNum = 1;
		public IObservable<MqttMessage> MqttMessageObservable { get; }

		public MqttService(ILogger<MqttService> logger, NeonConfig neonConfig)
		{
			_logger = logger;
			_config = neonConfig.ServicesConfig.MqttConfig;
			MqttMessageObservable = new ReplaySubject<MqttMessage>();
		}

		public async Task<bool> Start()
		{
			if (_config.UseEmbeddedServer)
			{
				_logger.LogInformation($"Starting MQTT in local mode on port {_config.EmbeddedServerPort}");

				var serverOptions = new MqttServerOptionsBuilder()
					.WithConnectionBacklog(100)
					.WithDefaultEndpointPort(_config.EmbeddedServerPort)
					.WithDefaultEndpointBoundIPAddress(new IPAddress(0)).Build();

				_mqttServer = new MqttFactory().CreateMqttServer();
				await _mqttServer.StartAsync(serverOptions);
				_logger.LogInformation($"Server started on port {_config.EmbeddedServerPort}");
			}
			await ConnectToServer();
			return true;
		}

		private async Task ConnectToServer()
		{
			var clientOptions = new MqttClientOptionsBuilder().WithClientId("Neon-Server");
			if (_config.UseEmbeddedServer)
			{
				clientOptions = clientOptions.WithTcpServer("127.0.0.1", _config.EmbeddedServerPort);
			}
			else
			{
				clientOptions = clientOptions.WithTcpServer(_config.Client.Hostname, _config.Client.Port);
			}

			_mqttClient = new MqttFactory().CreateMqttClient();
			var options = clientOptions.Build();

			_logger.LogInformation($"Connecting to server ");

			_mqttClient.UseDisconnectedHandler(async args =>
			{
				_logger.LogInformation($"Disconnected from server, will retry in 5 seconds");
				await Task.Delay(5000);

				if (_clientConnectionRetryNum <= 3)
				{
					if (_mqttClient != null)
					{
						await _mqttClient.ConnectAsync(options);
						_logger.LogInformation($"Reconnected to server...");
						_clientConnectionRetryNum++;
					}

				}
			});

			_mqttClient.UseApplicationMessageReceivedHandler(args =>
			{
				OnMessageReceived(args.ApplicationMessage.Topic, Encoding.UTF8.GetString(args.ApplicationMessage.Payload));
			});

			await _mqttClient.ConnectAsync(options);
			await SubscribeTopic("/neon");
			_logger.LogInformation($"MQTT Client Connected");
		}

		private void OnMessageReceived(string topic, string payload)
		{
			_logger.LogDebug($"Received message from topic {topic} - {payload}");
			((ReplaySubject<MqttMessage>)MqttMessageObservable).OnNext(new MqttMessage() { Topic = topic, Payload = payload });
		}

		public async Task<bool> Stop()
		{
			if (_mqttClient != null)
			{
				await _mqttClient?.DisconnectAsync();
				_mqttClient?.Dispose();
			}

			if (_mqttServer != null)
			{
				await _mqttServer.StopAsync();
			}
			return true;

		}


		public async Task<bool> SubscribeTopic(string topicName)
		{
			if (_mqttClient.IsConnected)
			{
				var subscribeResult = await _mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topicName).Build());
				_logger.LogDebug($"Subscribed on topic {topicName}");
				return true;
			}

			return false;
		}

		public Task<bool> SendMessage(MqttMessage message)
		{
			if (_mqttClient.IsConnected)
			{

				_logger.LogDebug($"Sending message to topic {message.Topic} - {message.Payload}");

				var msg = new MqttApplicationMessageBuilder()
					.WithTopic(message.Topic)
					.WithPayload(message.Payload)
					.WithExactlyOnceQoS()
					.WithRetainFlag()
					.Build();

				_ = Task.Factory.StartNew(async () => { await _mqttClient.PublishAsync(msg); });

				return Task.FromResult<bool>(true);
			}

			return Task.FromResult(false);
		}
	}
}
