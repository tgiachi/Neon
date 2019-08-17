using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using Neon.Api.Attributes;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.Services;

namespace Neon.Engine.Services
{

	[NeonService("MQTT Service", "Connect to MQTT queue")]
	public class MqttService : IMqttService
	{
		private readonly ILogger _logger;
		private readonly NeonConfig _config;
		private IMqttClient _mqttClient = null;
		public MqttService(ILogger<MqttService> logger, NeonConfig neonConfig)
		{
			_logger = logger;
			_config = neonConfig;
		}

		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public async Task<bool> Stop()
		{
			if (_mqttClient != null)
			{
				await _mqttClient?.DisconnectAsync();
				_mqttClient?.Dispose();
			}
			return true;

		}
	}
}
