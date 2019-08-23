using Humanizer;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Data.Config.Root;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using Neon.Engine.Data;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Neon.Engine.Components
{

	[NeonComponent("sunset", "v1.0.0.0", "SUN", typeof(SunSetConfig))]
	public class SunComponent : AbstractNeonComponent<SunSetConfig>
	{
		private HttpClient _httpClient = new HttpClient();
		private HomeConfig _homeConfig;
		public SunComponent(ILoggerFactory loggerFactory, IIoTService ioTService, NeonConfig neonConfig, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{
			_homeConfig = neonConfig.HomeConfig;
		}


		[ComponentPollRate(30)]
		public override async Task Poll()
		{
			var lat = _homeConfig.CoordinateConfig.Latitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
			var lon = _homeConfig.CoordinateConfig.Longitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");

			var rs = await _httpClient.GetAsync(
				$"https://api.sunrise-sunset.org/json?lat={lat}&lng=-{lon}=today");

			if (rs.IsSuccessStatusCode)
			{
				ParseResult(await rs.Content.ReadAsStringAsync());
			}

			await base.Poll();

		}

		private void ParseResult(string content)
		{
			var entity = BuildEntity<SunEvent>();
			var parsedEntity = JsonConvert.DeserializeObject<SunSetData>(content);

			entity.Date = DateTime.Now.Date.ToShortDateString();
			entity.SunRise = DateTime.ParseExact(parsedEntity.Results.Sunrise, "h:mm:ss tt", CultureInfo.InvariantCulture);
			entity.SunSet = DateTime.ParseExact(parsedEntity.Results.Sunset, "h:mm:ss tt", CultureInfo.InvariantCulture);
			entity.DayLength = DateTime
				.ParseExact(parsedEntity.Results.DayLength, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;

			PublishEntity(entity);
		}
	}
}
