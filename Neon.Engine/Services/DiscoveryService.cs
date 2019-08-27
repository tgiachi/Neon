using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Makaretu.Dns;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Data.Discovery;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using Zeroconf;

namespace Neon.Engine.Services
{
	[NeonService("Discovery Service", "Service for enabled network discovery", Int16.MaxValue)]
	public class DiscoveryService : IDiscoveryService
	{
		private readonly string _uuid;
		private readonly ILogger _logger;
		private readonly DiscoveryConfig _discoveryConfig;
		private readonly ISchedulerService _schedulerService;
		private readonly List<DiscoveryListenerData> _discoveryListeneres;
		private List<DiscoveryDevice> _discoveredDevices = new List<DiscoveryDevice>();
		private readonly ServiceDiscovery _serviceAdvertiser = new ServiceDiscovery();
		public DiscoveryService(ILogger<DiscoveryService> logger, NeonConfig neonConfig, ISchedulerService schedulerService, List<DiscoveryListenerData> discoveryListeners)
		{
			_uuid = neonConfig.EngineConfig.Uuid;
			_discoveryListeneres = discoveryListeners;
			_discoveryConfig = neonConfig.ServicesConfig.DiscoveryConfig;
			_schedulerService = schedulerService;
			_logger = logger;
		}

		public Task<bool> Start()
		{
			if (!_discoveryConfig.EnabledDiscovery) return Task.FromResult(false);

			StartAdvertiser();

			_schedulerService.AddJob(StartDiscovery, 300, true);
			return Task.FromResult(true);
		}

		private void StartAdvertiser()
		{
			_logger.LogDebug($"Starting advertiser on local network");
			var neonService = new ServiceProfile("neon", "_neonhome._tcp", 5000);
			neonService.AddProperty("http", "5000");
			neonService.AddProperty("https", "5001");
			neonService.AddProperty("appVersion", AssemblyUtils.GetVersion());
			neonService.AddProperty("uuid", _uuid);

			_serviceAdvertiser.Advertise(neonService);
		}

		private async void StartDiscovery()
		{
			var domains = await ZeroconfResolver.BrowseDomainsAsync();
			var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));

			responses.ToList().ForEach(r =>
			{
				r.Services.ToList().ForEach(s =>
				{
					var service = new DiscoveryDevice()
					{
						IpAddress = r.IPAddress,
						Service = s.Key.Split('.')[0],
						Name = s.Key,
						Port = s.Value.Port,
						Ttl = s.Value.Ttl
					};

					s.Value.Properties.ToList().ForEach(k =>
					{
						foreach (var (key, value) in k.ToList())
						{
							if (!service.Properties.ContainsKey(key))
								service.Properties.Add(key, value);
						}
					});

					_discoveredDevices.Add(service);
				});
			});

			_discoveredDevices = _discoveredDevices.OrderBy(device => device.Name).ToList();
		}

		public Task<bool> Stop()
		{
			_serviceAdvertiser.Dispose();
			return Task.FromResult(true);

		}
	}
}
