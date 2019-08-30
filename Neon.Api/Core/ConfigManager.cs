using Autofac;
using Neon.Api.Attributes.Config;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using Serilog;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;

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

			
			if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(),configFileName)))
			{
				_configFullPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
				_isConfigFound = true;
			}

			if (!_isConfigFound)
			{
				_configFullPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
			}

			if (_neonManager.IsRunningInDocker)
			{

				_configFullPath = Path.Combine("/neon", configFileName);

				if (File.Exists(_configFullPath))
					_isConfigFound = true;
			}

			_logger.Information($"Loading config [is Docker = {_neonManager.IsRunningInDocker}]  config found: {_isConfigFound}");


			if (!_isConfigFound)
			{
				_config = new NeonConfig();
				SaveConfig();
			}
			else
				DeserializeConfig();

			CheckEnvVariables();

			if (_neonManager.IsRunningInDocker)
			{
				_config.EngineConfig.HomeDirectory = "/neon";
			}

			_containerBuilder.RegisterInstance(_config);
			SaveConfig();

			return true;
		}

		private void CheckEnvVariables()
		{
			_config.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(p =>
			  {
				  _logger.Debug($"Property {p.Name}");
				  ScanProperty(_config, p);
			  });

		}

		private void ScanProperty(object obj, PropertyInfo property)
		{
			if (property.PropertyType != typeof(string) && property.PropertyType != typeof(int) && property.PropertyType != typeof(Boolean))
			{
				property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(p =>
				  {
					  if (p.PropertyType != typeof(IList) && p.PropertyType.IsGenericType)
						  ScanProperty(property.GetValue(obj), p);
				  });
			}
			else
			{
				if (property.GetCustomAttribute<ConfigEnvVariableAttribute>() != null)
				{
					var attr = property.GetCustomAttribute<ConfigEnvVariableAttribute>();

					var envValue = Environment.GetEnvironmentVariable(attr.EnvName);
					_logger.Debug($"Set env variable {attr.EnvName} to property {property.Name}");

					property.SetValue(obj, envValue);
				}
			}
		}

		private void DeserializeConfig()
		{
			_config = File.ReadAllText(_configFullPath).FromYaml<NeonConfig>();
		}

		public void SaveConfig()
		{
			File.WriteAllText(_configFullPath, _config.ToYaml());
		}

	}
}
