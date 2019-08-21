using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Data.Mqtt;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using Neon.Engine.Data;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Automation
{
	[NeonComponent("sonoff_tasmoda", "v1.0.0.0", "AUTOMATION", typeof(SonoffTasmodaConfig))]
	public class SonoffTasmodaComponent : AbstractNeonComponent<SonoffTasmodaConfig>
	{
		private IMqttService _mqttService;

		public SonoffTasmodaComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IMqttService mqttService) : base(loggerFactory, ioTService)
		{
			_mqttService = mqttService;
		}


		public override Task<bool> Start()
		{
			_mqttService.SubscribeTopic("tele/+/+");
			_mqttService.SubscribeTopic("stat/+/+");
			_mqttService.SubscribeTopic("cmnd/+/+");
			_mqttService.SubscribeTopic("pasquicci/+");


			_mqttService.MqttMessageObservable.Subscribe(OnMqttMessage);
			return base.Start();
		}

		private void OnMqttMessage(MqttMessage obj)
		{
			if (obj.Topic.StartsWith("tele"))
				ParseTeleStatus(obj.Topic, obj.Payload);

			if (obj.Topic.StartsWith("cmnd"))
				ParseCmnd(obj.Topic, obj.Payload);
			//if (obj.Topic.StartsWith("pasquicci"))
			//	await Toggle("Salotto", 2);
		}

		private void ParseCmnd(string topic, string message)
		{
			var entity = BuildEntity<SonoffTasmodaEvent>();
			var splittedTele = topic.Split("/");

			entity.Name = splittedTele[1];

			entity = IoTService.GetEntityByType<SonoffTasmodaEvent>(entity.Name, entity.EntityType);

			if (entity != null)
			{
				var entry = entity.PowerStatuses.FirstOrDefault(e => e.PowerName == splittedTele[2]);

				if (entry != null)
				{
					entry.Status = message;
				}
				else
				{
					entity.PowerStatuses.Add(new SonoffTasmodaPowerStatus()
					{
						PowerName = splittedTele[2],
						Status = message
					});
				}

				entity.PowerCount = entity.PowerStatuses.Count;

				PublishEntity(entity);
			}
		}

		private void ParseTeleStatus(string topic, string message)
		{
			var entity = BuildEntity<SonoffTasmodaEvent>();
			var splittedTele = topic.Split("/");

			entity.Name = splittedTele[1];

			if (splittedTele[2] == "LWT")
			{
				entity.Status = message;
			}
			else if (splittedTele[2] == "STATE")
			{
				var sonoffStatus = JsonConvert.DeserializeObject<SonoffTasmodaStatusData>(message);

				if (!string.IsNullOrEmpty(sonoffStatus.Power1))
					entity.PowerStatuses.Add(new SonoffTasmodaPowerStatus() { PowerName = "POWER1", Status = sonoffStatus.Power1 });

				if (!string.IsNullOrEmpty(sonoffStatus.Power2))
					entity.PowerStatuses.Add(new SonoffTasmodaPowerStatus() { PowerName = "POWER2", Status = sonoffStatus.Power1 });

				if (!string.IsNullOrEmpty(sonoffStatus.Power3))
					entity.PowerStatuses.Add(new SonoffTasmodaPowerStatus() { PowerName = "POWER3", Status = sonoffStatus.Power1 });

				if (!string.IsNullOrEmpty(sonoffStatus.Power4))
					entity.PowerStatuses.Add(new SonoffTasmodaPowerStatus() { PowerName = "POWER4", Status = sonoffStatus.Power1 });

				entity.PowerCount = entity.PowerStatuses.Count;
			}

			PublishEntity(entity);
		}

		public async Task<bool> SetStatus(string deviceName, int channel, bool active)
		{
			return await _mqttService.SendMessage(new MqttMessage()
			{
				Payload = active ? "ON" : "OFF",
				Topic = $"cmnd/{deviceName}/POWER{channel}"
			});
		}

		public async Task<bool> Toggle(string deviceName, int channel)
		{
			await SetStatus(deviceName, channel, true);
			await Task.Delay(2000);
			await SetStatus(deviceName, channel, false);
			return true;
		}

		[ComponentPollRate(0, IsEnabled = false)]
		public override Task Poll()
		{
			return base.Poll();
		}
	}
}
