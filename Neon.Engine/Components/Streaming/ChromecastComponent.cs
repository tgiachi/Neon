using GoogleCast;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Streaming
{
	[NeonComponent("chromecast", "v1.0.0.0", "STREAMING", typeof(ChromecastConfig))]
	public class ChromecastComponent : AbstractNeonComponent<ChromecastConfig>
	{
		private bool _devicesFound;

		public ChromecastComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}


		[ComponentPollRate(60)]
		public override async Task Poll()
		{
			if (!_devicesFound)
			{
				await SearchDevices();

				_devicesFound = true;
			}
			await base.Poll();
		}

		private async Task SearchDevices()
		{
			var deviceFinder = new DeviceLocator();
			Logger.LogInformation("Starting search devices");

			var devices = await deviceFinder.FindReceiversAsync();
			if (devices != null)
			{
				devices = devices.ToList();
				Logger.LogInformation($"Found {devices.Count()} found!");
				foreach (var receiver in devices.ToList())
				{
					var entity = BuildEntity<ChromecastEvent>();
					entity.DeviceId = receiver.Id;
					entity.IpAddress = receiver.IPEndPoint.ToString();
					entity.Name = receiver.FriendlyName;

					PublishEntity(entity);
				}
			}
		}
	}
}
