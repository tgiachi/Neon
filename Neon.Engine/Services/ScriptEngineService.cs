using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using System.IO;
using System.Threading.Tasks;

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

			//https://docs.microsoft.com/en-us/powershell/developer/hosting/creating-an-initialsessionstate
			//https://docs.microsoft.com/en-us/powershell/developer/hosting/creating-a-constrained-runspace
			//https://www.powershellmagazine.com/2014/03/18/writing-a-powershell-module-in-c-part-1-the-basics/

			return Task.FromResult(true);
		}


		//public Runspace GetRunspace()
		//{
		//	var state = InitialSessionState.CreateDefault2();
		//	if (_modules != null)
		//	{
		//		state.ImportPSModule(_modules);
		//	}
		//	state.ExecutionPolicy = ExecutionPolicy.RemoteSigned;
		//	state.Providers.Remove("Registry", null);
		//	state.Providers.Remove("FileSystem", null);
		//	if (_variables != null)
		//	{
		//		foreach (var variable in _variables)
		//		{
		//			state.Variables.Add(new SessionStateVariableEntry(variable.Key, variable.Value, variable.Key, ScopedItemOptions.Constant));
		//		}
		//	}
		//	var runspace = RunspaceFactory.CreateRunspace(_host, state);
		//	runspace.Open();

		//	return runspace;
		//}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);

		}
	}
}
