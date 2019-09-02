using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Services;
using System;

namespace Neon.Engine.Std
{
	[ScriptModule]
	public class RoutinesScriptModule
	{
		private readonly IRoutineService _routineService;
		private readonly ILogger _logger;
		public RoutinesScriptModule(IRoutineService routineService, ILogger<ScriptEngineService> logger)
		{
			_logger = logger;
			_routineService = routineService;
		}

		[ScriptFunction("add_routine", "Add routine")]
		public bool AddRoutines(string name, Action<object[]> function, params object[] args)
		{

			_logger.LogDebug($"Adding Routine {name} params count {args.Length} - delegate params count {function.Method.GetParameters().Length}");
			return _routineService.AddRoutine(name, () =>
			{
				//		if (args.Length > 0)
				//function.Method.Invoke(function.Target, args);
				//			else
				function.Invoke(args);
			});
		}

		[ScriptFunction("exec_routine", "Exec routine")]
		public void ExecuteRoutine(string name)
		{
			_logger.LogDebug($"Executing Routine {name}");
			_routineService.ExecuteRoutine(name);
		}
	}
}
