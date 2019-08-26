using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.WebHook;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Api.Interfaces.WebHook;
using Neon.Api.Utils;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using Neon.Engine.Data;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Streaming
{

	[WebHookReceiver("plex")]
	[NeonComponent("plex_hook", "v1.0.0.0", "STREAMING", typeof(PlexHookConfig))]
	public class PlexHookComponent : AbstractNeonComponent<PlexHookConfig>, IWebHookReceiver
	{

		public PlexHookComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}


		[ComponentPollRate(0, IsEnabled = false)]
		public override Task Poll()
		{
			return base.Poll();
		}

		public void OnHook(string payload)
		{
			var plexData = payload.FromJson<PlexHookData>();

			var entity = BuildEntity<PlexHookEvent>();

			entity.Event = plexData.Event;
			entity.Guid = plexData.Metadata.Guid;
			entity.Key = plexData.Metadata.Key;
			entity.LibrarySectionTitle = plexData.Metadata.LibrarySectionTitle;
			entity.Rating = plexData.Metadata.Rating;
			entity.Title = plexData.Metadata.Title;
			entity.RatingKey = plexData.Metadata.RatingKey;
			entity.LibrarySectionType = plexData.Metadata.LibrarySectionType;

			PublishEntity(entity);

		}
	}
}
