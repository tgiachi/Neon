using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Engine.Services;
using System;

namespace Neon.Engine.Std
{
	[ScriptModule]
	public class LogScriptModule
	{
		private readonly ILogger _logger;
		private readonly ScriptEngineConfig _config;

		public LogScriptModule(ILogger<ScriptEngineService> logger, NeonConfig neonConfig)
		{
			_logger = logger;
			_config = neonConfig.ServicesConfig.ScriptEngineConfig;
		}

		[ScriptFunction("log_info", "Log info")]
		public void LogInfo(string text, params object[] objects)
		{
			if (_config.WriteOnLogOutput)
				_logger.LogInformation(text, objects);

			if (_config.WriteOnConsoleOutput)
				Console.WriteLine(text, objects);
		}

		[ScriptFunction("log_warning", "Log warning")]
		public void LogWarning(string text, params object[] objects)
		{
			if (_config.WriteOnLogOutput)
				_logger.LogWarning(text, objects);

			if (_config.WriteOnConsoleOutput)
				Console.WriteLine(text, objects);
		}

		[ScriptFunction("log_error", "Log error")]
		public void LogError(string text, params object[] objects)
		{
			if (_config.WriteOnLogOutput)
				_logger.LogError(text, objects);

			if (_config.WriteOnConsoleOutput)
				Console.WriteLine(text, objects);
		}

	}
}
