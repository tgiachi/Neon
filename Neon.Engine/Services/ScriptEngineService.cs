using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Data.ScriptEngine;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using NLua;
using NLua.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("Script Engine", "Script engine", 5)]
	public class ScriptEngineService : IScriptEngineService
	{
		private const string BootstrapFilename = "bootstrap.lua";

		public List<ScriptFunctionData> Functions { get; }
		public List<ScriptEngineVariable> Variables { get; }

		private readonly IFileSystemManager _fileSystemManager;
		private readonly INeonManager _neonManager;
		private readonly ILogger _logger;
		private readonly ScriptEngineConfig _config;
		private readonly NeonConfig _neonConfig;
		private readonly Lua _luaEngine = new Lua();
		private readonly List<LuaFunction> _functions = new List<LuaFunction>();

		public ScriptEngineService(ILogger<ScriptEngineService> logger,
			INeonManager neonManager,
			IFileSystemManager fileSystemManager,
			NeonConfig neonConfig)
		{
			_logger = logger;
			_neonManager = neonManager;
			_fileSystemManager = fileSystemManager;
			_neonConfig = neonConfig;
			_config = neonConfig.ServicesConfig.ScriptEngineConfig;
			Functions = new List<ScriptFunctionData>();
			Variables = new List<ScriptEngineVariable>();
		}

		public async Task<bool> Start()
		{

			_logger.LogInformation($"Initialize LUA Scripting engine");

			await LoadLua();
			ScanScriptsModules();

			_fileSystemManager.CreateDirectory(_config.ScriptsDirectory.DirectoryName);

			if (!_fileSystemManager.IsFileExists(Path.Combine(_config.ScriptsDirectory.DirectoryName, BootstrapFilename)))
			{
				_fileSystemManager.WriteToFile(Path.Combine(_config.ScriptsDirectory.DirectoryName, BootstrapFilename), "");
			}

			_logger.LogInformation($"Script directory: {_config.ScriptsDirectory.DirectoryName}");

			CheckModulesDirectory();

			AddDefaultVariables();

			LoadBootstrap();

			await Build();

			return true;
		}

		private void AddDefaultVariables()
		{
			AddVariable("home", _neonConfig.HomeConfig);
			AddVariable("system_config", _neonConfig);
		}

		private void CheckModulesDirectory()
		{
			var modulesDirectory = Path.Join(_config.ScriptsDirectory.DirectoryName,
				_config.ModulesDirectory.DirectoryName);

			var fullModulesDirectory = _fileSystemManager.BuildFilePath(modulesDirectory);

			_logger.LogInformation($"Modules directory {modulesDirectory}");

			_fileSystemManager.CreateDirectory(modulesDirectory);

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				fullModulesDirectory = fullModulesDirectory.Replace(@"\", @"\\");
			}

			_luaEngine.DoString($@"
			-- Update the search path
			local module_folder = '{fullModulesDirectory}'
			package.path = module_folder .. '?.lua;' .. package.path");
		}

		private void LoadBootstrap()
		{
			var bootstrapFile = Path.Combine(_config.ScriptsDirectory.DirectoryName, BootstrapFilename);

			LoadFile(_fileSystemManager.BuildFilePath(bootstrapFile), false);
		}

		private void ScanScriptsModules()
		{
			AssemblyUtils.GetAttribute<ScriptModuleAttribute>().ForEach(LoadScriptModule);
		}

		private void LoadScriptModule(Type moduleType)
		{
			try
			{
				var moduleObj = _neonManager.Resolve(moduleType);

				foreach (var methodInfo in moduleType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
				{
					var scriptFunctionAttribute = methodInfo.GetCustomAttribute<ScriptFunctionAttribute>();

					if (scriptFunctionAttribute != null)
					{
						_logger.LogInformation($"Module: {moduleType.Name} - {scriptFunctionAttribute.FunctionName} - {scriptFunctionAttribute.HelpText}");

						var functionData = new ScriptFunctionData()
						{
							FunctionName = scriptFunctionAttribute.FunctionName,
							OutputType = methodInfo.ReturnType.Name,
							ScriptModuleName = moduleType.Name,
							HelpText = scriptFunctionAttribute.HelpText
						};

						foreach (var parameter in methodInfo.GetParameters())
						{
							functionData.Parameters.Add(new ScriptFunctionParamData()
							{
								ParamOrder = parameter.Position,
								ParamName = parameter.Name,
								ParamType = parameter.ParameterType.Name

							});
						}

						RegisterFunction(scriptFunctionAttribute.FunctionName, moduleObj, methodInfo);

						Functions.Add(functionData);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during load script module: {moduleType.Name} - {ex}");
			}

		}

		private Task LoadLua()
		{
			_luaEngine.State.Encoding = Encoding.UTF8;
			_luaEngine.LoadCLRPackage();

			_luaEngine.HookException += _luaEngine_HookException;

			return Task.CompletedTask;

		}

		private void _luaEngine_HookException(object sender, NLua.Event.HookExceptionEventArgs args)
		{
			if (args.Exception is LuaException luaException)
			{
				_logger.LogError($"Error during execute LUA =>\n {FormatException(luaException)}");
			}
			else
			{
				_logger.LogError($"Error during execute LUA =>\n {args.Exception}");
			}
		}

		private string FormatException(LuaException e)
		{
			var source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
			return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
		}

		public void LoadFile(string fileName, bool immediateExecute)
		{
			try
			{
				_logger.LogInformation($"Loading file {fileName}...");
				var func = _luaEngine.LoadFile(fileName);

				_functions.Add(func);
				if (immediateExecute)
					func.Call();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during load file {fileName} => {ex}");
			}
		}

		public void RegisterFunction(string functionName, object obj, MethodInfo method)
		{
			_luaEngine.RegisterFunction(functionName, obj, method);
		}

		public object ExecuteCode(string code)
		{
			return _luaEngine.DoString(code);
		}

		public Task<bool> Build()
		{
			_functions.ForEach(f => { f.Call(); });

			return Task.FromResult(true);
		}

		public void AddVariable(string variable, object value)
		{
			_luaEngine[variable] = value;
			Variables.Add(new ScriptEngineVariable() { Key = variable, Value = value });
		}


		public Task<bool> Stop()
		{
			_functions.ForEach(f => f.Dispose());
			_luaEngine?.Dispose();
			return Task.FromResult(true);

		}
	}


}
