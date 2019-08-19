using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac;

using Neon.Api.Data.Config;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces;
using Neon.Api.Interfaces.Managers;
using Serilog;
using YamlDotNet.Serialization;

namespace Neon.Api.Core
{
	public class ConfigManager : IConfigManager
	{
		private static string configEnv = "NEON_CONFIG_FILE";
		private static string configFileName = "neon-config.yaml";

		private readonly ILogger _logger;
		private readonly INeonManager _neonManager;
		private readonly ContainerBuilder _containerBuilder;

		private NeonConfig _config;
		private string _configFullPath;
		private bool _isConfigFound;


		public NeonConfig Configuration => _config;

		public ConfigManager(ILogger logger, INeonManager neonManager, ContainerBuilder containerBuilder)
		{
			_containerBuilder = containerBuilder;
			_logger = logger;
			_neonManager = neonManager;
		}

		public bool LoadConfig()
		{
			if (Environment.GetEnvironmentVariable(configEnv) != null)
			{
				_configFullPath = Environment.GetEnvironmentVariable(configEnv);
				_isConfigFound = true;
			}

			if (File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + configFileName))
			{
				_configFullPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + configFileName;
				_isConfigFound = true;
			}

			if (!_isConfigFound)
			{
				_configFullPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + configFileName;

			}

			_logger.Information($"Loading config [is Docker = {_neonManager.IsRunningInDocker}]  config found: {_isConfigFound}");

			if (!_isConfigFound)
				SaveDefaultConfig();
			else
				DeserializeConfig();


			_containerBuilder.RegisterInstance(_config);

			return true;
		}

		private void DeserializeConfig()
		{
			_config = new Deserializer().Deserialize<NeonConfig>(File.ReadAllText(_configFullPath));
		}

		private void SaveDefaultConfig()
		{
			_config = new NeonConfig();

			File.WriteAllText(_configFullPath, new Serializer().Serialize(_config));

		}
	}
}
