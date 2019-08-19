using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;

namespace Neon.Engine.Services
{
	[NeonService("Script Engine", "Script engine", 3)]
	public class ScriptEngineService : IScriptEngineService
	{
		private const string BootstrapFilename = "bootstrap.ps1";

		private readonly IFileSystemManager _fileSystemManager;
		private readonly ILogger _logger;
		private readonly ScriptEngineConfig _config;

		public ScriptEngineService(ILogger<ScriptEngineService> logger, IFileSystemManager fileSystemManager, NeonConfig neonConfig)
		{
			_logger = logger;
			_fileSystemManager = fileSystemManager;
			_config = neonConfig.ServicesConfig.ScriptEngineConfig;
		}

		public Task<bool> Start()
		{
			_fileSystemManager.CreateDirectory(_config.Directory.DirectoryName);

			if (!_fileSystemManager.IsFileExists(Path.Combine(_config.Directory.DirectoryName, BootstrapFilename)))
			{
				_fileSystemManager.WriteToFile(Path.Combine(_config.Directory.DirectoryName, BootstrapFilename), "");
			}

			_logger.LogInformation($"Script directory: {_config.Directory.DirectoryName}");

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);

		}
	}
}
