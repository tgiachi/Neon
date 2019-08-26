using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;

namespace Neon.Engine.Components.Remote
{

	[NeonComponent("broadlink", "v1.0.0.0", "REMOTE", typeof(BroadlinkConfig))]
	public class BroadlinkComponent : AbstractNeonComponent<BroadlinkConfig>
	{
		public BroadlinkComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{
		}
	}
}
