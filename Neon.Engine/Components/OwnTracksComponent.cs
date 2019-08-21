using GeoCoordinatePortable;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Mqtt;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using Neon.Engine.Data;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Neon.Engine.Components
{
	[NeonComponent("owntracks", "v1.0.0.0", "LOCATION", typeof(OwnTrackConfig))]
	public class OwnTracksComponent : AbstractNeonComponent<OwnTrackConfig>
	{
		private readonly NeonConfig _config;
		private readonly IMqttService _mqttService;
		public OwnTracksComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IMqttService mqttService, NeonConfig neonConfig) : base(loggerFactory, ioTService)
		{
			_mqttService = mqttService;
			_config = neonConfig;
		}

		public override Task<bool> Start()
		{
			Logger.LogInformation($"Subscribing topic {Config.Topic}");
			_mqttService.MqttMessageObservable.Subscribe(OnMqttMessage);

			_mqttService.SubscribeTopic(Config.Topic);

			return base.Start();
		}

		private void OnMqttMessage(MqttMessage obj)
		{
			if (obj.Topic.StartsWith("owntracks"))
				ParseLocation(obj.Payload);
		}

		private void ParseLocation(string jsonMessage)
		{
			try
			{
				var entity = BuildEntity<OwnTracksEvent>();
				var data = JsonConvert.DeserializeObject<OwnTracksData>(jsonMessage);
				Logger.LogInformation($"Received location from ID: {data.Id}");
				entity.Name = data.Id;
				entity.TrackerId = data.Id;
				entity.AccuracyMeters = data.AccuracyMeters;
				entity.Altitude = data.Altitude;
				entity.BatteryLevel = data.BatteryLevel;
				entity.Pressure = data.Pressure;
				entity.Latitude = data.Latitude;
				entity.Longitude = data.Longitude;
				entity.DistanceFromHomeInMeters = GetDistance(data.Latitude, data.Longitude);

				PublishEntity(entity);

			}
			catch (Exception ex)
			{
				Logger.LogError($"Can't parse JSON from Owntracks {ex.Message}");
			}

		}

		private double GetDistance(double latitude, double longitude)
		{
			if (_config.HomeConfig.CoordinateConfig.Latitude == 0.1)
			{
				Logger.LogWarning($"Home location configuration is default, please update with your location");
				return 1000000;
			}

			var distance1 = new GeoCoordinate(latitude, longitude);
			var distance2 = new GeoCoordinate(_config.HomeConfig.CoordinateConfig.Latitude, _config.HomeConfig.CoordinateConfig.Longitude);

			return distance2.GetDistanceTo(distance1);
		}

		[ComponentPollRate(0, IsEnabled = false)]
		public override Task Poll()
		{
			return base.Poll();
		}
	}
}
