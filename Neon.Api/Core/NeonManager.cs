using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Neon.Api.Interfaces;
using Neon.Api.Utils;

namespace Neon.Api.Core
{

	/// <summary>
	/// Implementation of Neon Manager 
	/// </summary>
	public class NeonManager : INeonManager
	{
		private readonly ILogger _logger;
		private IContainer _container;
		private ContainerBuilder _containerBuilder;

		public ContainerBuilder ContainerBuilder => _containerBuilder;


		public NeonManager(ILogger<NeonManager> logger)
		{
			_logger = logger;
			_logger.LogDebug($"Pre-loading assemblies");
			AssemblyUtils.GetAppAssemblies();

			_containerBuilder = new ContainerBuilder();
			_containerBuilder.RegisterBuildCallback(container => { _logger.LogDebug($"Container is ready"); });
		}



		public Task<bool> Init()
		{

			return Task.FromResult(true);
		}

		public Task Start()
		{
			return Task.CompletedTask;

		}
	}
}
