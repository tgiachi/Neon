using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs.Streaming;
using Neon.Engine.Components.Events;
using RadarrSharp;
using System.Threading.Tasks;

namespace Neon.Engine.Components
{
	[NeonComponent("radarr", "v1.0.0.0", "STREAMING", typeof(RadarrConfig))]
	public class RadarrComponent : AbstractNeonComponent<RadarrConfig>
	{
		private bool _isConfigured = false;
		private RadarrClient _radarrClient;
		public RadarrComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
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
			var entity = BuildEntity<RadarrEvent>();
			var movies = await _radarrClient.Movie.GetMovies();
			entity.MoviesCount = movies.Count;
			PublishEntity(entity);
		}

		public override Task<bool> Start()
		{
			if (Config.Api.ApiKey == "change_me")
				ThrowComponentNeedConfiguration("ApiKey");

			if (Config.Server.Hostname == "change_me")
				ThrowComponentNeedConfiguration("Hostname", "Port");

			_isConfigured = true;

			_radarrClient = new RadarrClient(Config.Server.Hostname, Config.Server.Port, Config.Api.ApiKey);


			return base.Start();
		}
	}
}
