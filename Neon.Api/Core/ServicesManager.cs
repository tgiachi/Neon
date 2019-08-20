using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Services;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Neon.Api.Core
{
	public class ServicesManager : IServicesManager
	{
		private readonly ILogger _logger;
		private readonly INeonManager _neonManager;
		private readonly SortedDictionary<int, List<Type>> _orderedService = new SortedDictionary<int, List<Type>>();
		private readonly Dictionary<Guid, INeonService> _services = new Dictionary<Guid, INeonService>();

		public ServicesManager(ILogger<ServicesManager> logger, INeonManager neonManager)
		{
			_logger = logger;
			_neonManager = neonManager;

			SortServices();
		}

		public async Task<bool> Start()
		{

			foreach (var entry in _orderedService)
			{
				foreach (var service in entry.Value)
				{
					var result = await StartService(service);

					if (result.Status == ServiceStatus.Error)
					{
						_logger.LogError($"Error during starting service {service.Name}: {result.Error.Message}");
					}
				}
			}

			return true;
		}

		public async Task<bool> Stop()
		{
			foreach (var entry in _services)
			{
				await entry.Value.Stop();
			}

			return true;
		}


		private async Task<ServiceInfo> StartService(Type type)
		{
			var sw = new Stopwatch();
			sw.Start();

			var serviceAttribute = type.GetCustomAttribute<NeonServiceAttribute>();
			_logger.LogInformation($"Starting service {serviceAttribute.Name} Order = {serviceAttribute.LoadOrder} ");

			var serviceInfo = new ServiceInfo()
			{
				Name = serviceAttribute.Name,
				Description = serviceAttribute.Description
			};


			try
			{
				var service = _neonManager.Resolve(AssemblyUtils.GetInterfaceOfType(type)) as INeonService;

				await service.Start();

				_services.Add(serviceInfo.Id, service);

				serviceInfo.Status = ServiceStatus.Started;

				sw.Stop();

				_logger.LogInformation($"Service {serviceAttribute.Name} started in {sw.Elapsed.TotalSeconds} seconds ");


				return serviceInfo;
			}
			catch (Exception e)
			{

				serviceInfo.Error = e;
				serviceInfo.Status = ServiceStatus.Error;
				return serviceInfo;
			}

		}

		private void SortServices()
		{
			_neonManager.AvailableServices.ForEach(s =>
			{
				var serviceAttribute = s.GetCustomAttribute<NeonServiceAttribute>();

				if (!_orderedService.ContainsKey(serviceAttribute.LoadOrder))
					_orderedService.Add(serviceAttribute.LoadOrder, new List<Type>());

				_orderedService[serviceAttribute.LoadOrder].Add(s);

			});
		}
	}
}
