using DarkSky.Services;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Data.Config.Root;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using System.Threading.Tasks;
using DarkSky.Models;

namespace Neon.Engine.Components.Weather
{
	[NeonComponent("weather_darksky", "v1.0.0.0", "WEATHER", typeof(DarkSkyConfig))]
	public class DarkSkyComponent : AbstractNeonComponent<DarkSkyConfig>
	{
		private DarkSkyService _darkSkyService;
		private HomeConfig _homeConfig;
		public DarkSkyComponent(ILoggerFactory loggerFactory, IIoTService ioTService, NeonConfig neonConfig, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{
			_homeConfig = neonConfig.HomeConfig;
		}

		public override Task<bool> Start()
		{
			if (string.IsNullOrEmpty(Config.ApiConfig.ApiKey) || Config.ApiConfig.ApiKey == "none")
				ThrowComponentNeedConfiguration(nameof(Config.ApiConfig.ApiKey));

			_darkSkyService = new DarkSkyService(Config.ApiConfig.ApiKey);


			return base.Start();
		}

		[ComponentPollRate(30)]
		public override async Task Poll()
		{
			var forecast = await _darkSkyService.GetForecast(_homeConfig.CoordinateConfig.Latitude,
				_homeConfig.CoordinateConfig.Longitude, new OptionalParameters()
				{
					LanguageCode = Config.LanguageCode,
					MeasurementUnits = Config.MeasurementUnits
				});

			if (forecast.IsSuccessStatus)
			{
				var entity = BuildEntity<WeatherEvent>();

				entity.Icon = forecast.Response.Currently.Icon.ToString();
				entity.Summary = forecast.Response.Currently.Summary;

				if (forecast.Response.Currently.Temperature != null)
					entity.Temperature = forecast.Response.Currently.Temperature.Value;

				if (forecast.Response.Currently.Humidity != null)
					entity.Humidity = forecast.Response.Currently.Humidity.Value * 100;

				if (forecast.Response.Currently.Pressure != null)
					entity.Pressure = forecast.Response.Currently.Pressure.Value;

				if (forecast.Response.Currently.PrecipProbability != null)
					entity.PrecipProbability = forecast.Response.Currently.PrecipProbability.Value * 100;

				entity.PrecipType = forecast.Response.Currently.PrecipType.ToString();

				PublishEntity(entity);
			}
			else
			{
				Logger.LogWarning("Error during get forecast");
			}


			await base.Poll();
		}
	}
}
