using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Data.Scheduler;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Broadlink.NET;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using Neon.Engine.Vaults;
using System;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Remote
{

	[NeonComponent("broadlink", "v1.0.0.0", "REMOTE", typeof(BroadlinkConfig))]
	public class BroadlinkComponent : AbstractNeonComponent<BroadlinkConfig>
	{
		private BroadlinkVault _broadlinkVault;
		private Client _broadlinkClient = new Client();
		public BroadlinkComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}

		public override Task<bool> Init(object config)
		{
			_broadlinkVault = LoadVault<BroadlinkVault>();
			return base.Init(config);
		}

		public override async Task<bool> Start()
		{
			if (_broadlinkVault.Devices.Count == 0)
			{
				Logger.LogInformation($"Starting Broadlink discovery");
				await StartDiscovery();
			}
			return await base.Start();
		}

		private async Task StartDiscovery()
		{

			try
			{
				var devices = await _broadlinkClient.DiscoverAsync();

				devices.ForEach(async d =>
				{
					var rmDevice = d as RMDevice;
					var entity = BuildEntity<BroadlinkDeviceEvent>();

					Logger.LogDebug($"Found Broadlink {d.DeviceId}");

					await rmDevice.AuthorizeAsync();

					var vaultConfig = new BroadlinkDeviceData()
					{
						MacAddress = BitConverter.ToString(rmDevice.MacAddress),
						DeviceId = BitConverter.ToString(rmDevice.DeviceId),
						IpAddress = d.LocalIPEndPoint.ToString(),
						EncryptKey = BitConverter.ToString(rmDevice.EncryptionKey)
					};

					entity.DeviceType = rmDevice.DeviceType;
					entity.MacAddress = vaultConfig.MacAddress;
					entity.Name = $"Broadlink_{entity.MacAddress}";

					PublishEntity(entity);
				});

				SaveVault(_broadlinkVault);
			}
			catch (Exception e)
			{
				Logger.LogError($"Error during discovery: {e.Message}");
			}
			
		}

		[ComponentPollRate((int)SchedulerServicePollingEnum.LongPolling)]
		public override async Task Poll()
		{
			if (_broadlinkVault.Devices.Count == 0)
				await StartDiscovery();

			await base.Poll();
		}
	}
}
