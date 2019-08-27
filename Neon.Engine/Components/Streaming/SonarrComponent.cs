using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs.Streaming;
using Neon.Engine.Components.Events;
using SonarrSharp;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Streaming
{

	[NeonComponent("sonarr", "v1.0.0.0", "STREAMING", typeof(SonarrConfig))]
	public class SonarrComponent : AbstractNeonComponent<SonarrConfig>
	{
		private SonarrClient _sonarrClient;
		private bool _isConfigured = false;
		public SonarrComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}


		[ComponentPollRate(60)]
		public override Task Poll()
		{
			if (_isConfigured)
				CollectData();
			return base.Poll();
		}

		private async void CollectData()
		{
			var entity = BuildEntity<SonarrEvent>();
			var series = await _sonarrClient.Series.GetSeries();
			entity.SeriesCount = series.Count;
			PublishEntity(entity);
		}

		public override Task<bool> Start()
		{
			if (Config.Api.ApiKey == "change_me")
				ThrowComponentNeedConfiguration("ApiKey");

			if (Config.Server.Hostname == "change_me")
				ThrowComponentNeedConfiguration("Hostname", "Port");

			_isConfigured = true;

			_sonarrClient = new SonarrClient(Config.Server.Hostname, Config.Server.Port, Config.Api.ApiKey);


			return base.Start();
		}
	}
}
