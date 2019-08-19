using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.NoSql;
using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.NoSql;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("NoSql Service", "Manage NoSQL connectors", 1)]
	public class NoSqlService : INoSqlService
	{
		private readonly Dictionary<string, Type> _nosqlConnectors = new Dictionary<string, Type>();
		private readonly ILogger _logger;
		private readonly INeonManager _neonManager;

		public NoSqlService(ILogger<NoSqlService> logger, INeonManager neonManager)
		{
			_logger = logger;
			_neonManager = neonManager;
		}

		public Task<bool> Start()
		{
			AssemblyUtils.GetAttribute<NoSqlConnectorAttribute>().ForEach(t =>
			{
				var attr = t.GetCustomAttribute<NoSqlConnectorAttribute>();
				_nosqlConnectors.Add(attr.Name, t);
				_logger.LogInformation($"Adding NoSql connector {attr.Name} [{t.Name}]");
			});

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_nosqlConnectors.Clear();
			return Task.FromResult(true);
		}

		public INoSqlConnector GetNoSqlConnector(string name)
		{
			return _neonManager.Resolve(_nosqlConnectors[name]) as INoSqlConnector;
		}
	}
}
