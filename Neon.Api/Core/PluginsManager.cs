using Neon.Api.Data.Config.Plugins;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neon.Api.Core
{
	public class PluginsManager : IPluginsManager
	{
		private readonly ILogger _logger;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly string _pluginsDirectory;
		private string _fullPluginDirectory;

		private readonly Dictionary<PluginConfig, Assembly> _pluginsAssemblies = new Dictionary<PluginConfig, Assembly>();

		public PluginsManager(ILogger logger, IFileSystemManager fileSystemManager, NeonConfig neonConfig)
		{
			_logger = logger.ForContext<PluginsManager>();
			_fileSystemManager = fileSystemManager;
			_pluginsDirectory = neonConfig.ServicesConfig.PluginsConfig.Directory.DirectoryName;
		}

		public void Start()
		{
			_logger.Information($"Starting Plugin manager - Plugin directory: {_pluginsDirectory}");
			_fileSystemManager.CreateDirectory(_pluginsDirectory);
			_fullPluginDirectory = _fileSystemManager.BuildFilePath(_pluginsDirectory);

			var templateConfig = new PluginConfig();

			_fileSystemManager.WriteToFile("template_plugin.yaml", templateConfig);

			ScanPlugins();
			LoadPlugins();
		}

		private void ScanPlugins()
		{
			var files = Directory.GetFileSystemEntries(_fullPluginDirectory, "*.yaml", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				LoadPlugin(file);
			}
		}

		private void LoadPlugins()
		{
			_pluginsAssemblies.Values.ToList().ForEach(p =>
			{
				_logger.Debug($"Adding {p.GetName().Name} to assemblies cache");
				AssemblyUtils.AddAssemblyToCache(p);
			});
		}

		private void LoadPlugin(string config)
		{
			try
			{
				var pluginConfig = File.ReadAllText(config).FromYaml<PluginConfig>();
				var pluginDirectory = Path.GetDirectoryName(config);
				_logger.Information($"Loading {pluginConfig.PluginInfoConfig.PluginName} v{pluginConfig.PluginInfoConfig.PluginVersion}");

				var assembly = Assembly.LoadFile(Path.Combine(pluginDirectory, pluginConfig.PluginDllName));

				_logger.Information($"Plugin {pluginConfig.PluginInfoConfig.PluginName} v{pluginConfig.PluginInfoConfig.PluginVersion} loaded!");

				_pluginsAssemblies.Add(pluginConfig, assembly);

			}
			catch (Exception ex)
			{
				_logger.Error($"Error during load plugin {config}: {ex.Message}");
			}

		}
	}
}
