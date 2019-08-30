using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Data.ScriptEngine;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jint.Parser;
using Jint.Runtime;
using Jint.Runtime.Interop;

namespace Neon.Engine.Services
{
	[NeonService("Script Engine", "Script engine", 9)]
	public class ScriptEngineService : IScriptEngineService
	{
		private const string BootstrapFilename = "bootstrap.js";
		public List<ScriptFunctionData> Functions { get; }
		public List<ScriptEngineVariable> Variables { get; }

		private readonly IFileSystemManager _fileSystemManager;
		private readonly Dictionary<string, string> _filesContents = new Dictionary<string, string>();
		private readonly INeonManager _neonManager;
		private readonly ILogger _logger;
		private readonly ScriptEngineConfig _config;
		private readonly NeonConfig _neonConfig;
		private Jint.Engine _engine;

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

			_logger.LogInformation($"Initialize JS Scripting engine");

			await LoadJs();
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

		private Task LoadJs()
		{
			_engine = new Jint.Engine(options =>
			{
				options.Culture(CultureInfo.CurrentCulture);
				options.DebugMode();
			});
				
			return Task.CompletedTask;

		}

		public void LoadFile(string fileName, bool immediateExecute)
		{
			try
			{
				_logger.LogInformation($"Loading file {fileName}...");
				_filesContents.Add(fileName, File.ReadAllText(fileName));
				if (immediateExecute)
					_engine.Execute(_filesContents[fileName]);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during load file {fileName} => {ex}");
			}
		}

		public void RegisterFunction(string functionName, object obj, MethodInfo method)
		{
			_engine.SetValue(functionName, CreateJsEngineDelegate(obj, method));
		}

		private Delegate CreateJsEngineDelegate(object obj, MethodInfo method)
		{
			return method.CreateDelegate(Expression.GetDelegateType(
				(from parameter in method.GetParameters() select parameter.ParameterType)
				.Concat(new[] { method.ReturnType })
				.ToArray()), obj);
		}

		public object StatementToObject(string code)
		{
			return _engine.Execute(code).GetCompletionValue().ToObject();
		}

		public object ExecuteCode(string code)
		{
			try
			{
				return _engine.Execute(code).GetCompletionValue().ToObject();

			}
			catch (JavaScriptException ex)
			{
				_logger.LogError($"Error during execute code '{code.Substring(0, code.Length / 2)}' line: {ex.LineNumber} column: {ex.Column} error: {ex.Error} ");

				throw;
			}
		}

		public Task<bool> Build()
		{
			foreach (var s in _filesContents.Values.ToList())
			{
				ExecuteCode(s);
			}

			return Task.FromResult(true);
		}

		public void AddVariable(string variable, object value)
		{
			_engine.SetValue(variable, value);
			Variables.Add(new ScriptEngineVariable() { Key = variable, Value = value });
		}


		public Task<bool> Stop()
		{
			_engine = null;
			return Task.FromResult(true);

		}
	}


}
