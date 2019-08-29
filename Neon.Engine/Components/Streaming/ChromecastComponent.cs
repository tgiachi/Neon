using GoogleCast;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using System.Linq;
using System.Threading.Tasks;
using Neon.Api.Attributes.Discovery;
using Neon.Api.Data.Discovery;
using Neon.Api.Interfaces.Discovery;

namespace Neon.Engine.Components.Streaming
{
	[DiscoveryService("_googlecast")]
	[NeonComponent("chromecast", "v1.0.0.0", "STREAMING", typeof(ChromecastConfig))]
	public class ChromecastComponent : AbstractNeonComponent<ChromecastConfig>, IDiscoveryDevice
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
					AddDevice(receiver.Id, receiver.IPEndPoint.ToString(), receiver.FriendlyName);
				}
			}
		}

		public void OnDeviceDiscovered(DiscoveryDevice device)
		{
			AddDevice(device.Properties["id"], $"{device.IpAddress}:{device.Port}", device.Properties["fn"] );
			
		}

		private void AddDevice(string id, string ipAddress, string name)
		{
			var entity = BuildEntity<ChromecastEvent>();
			entity.DeviceId = id;
			entity.IpAddress = ipAddress;
			entity.Name = name;

			PublishEntity(entity);
		}
	}
}
