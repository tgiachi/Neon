using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Services;
using NLua;

namespace Neon.Engine.Std
{
	[ScriptModule]
	public class RoutinesScriptModule
	{
		private readonly IRoutineService _routineService;

		public RoutinesScriptModule(IRoutineService routineService)
		{
			_routineService = routineService;
		}

		[ScriptFunction("add_routine", "Add routine")]
		public bool AddRoutines(string name, LuaFunction function, params object[] args)
		{
			return _routineService.AddRoutine(name, () => { function.Call(args); });
		}

		[ScriptFunction("execute_routine", "Add routine")]
		public void ExecuteRoutine(string name)
		{
			_routineService.ExecuteRoutine(name);
		}
	}
}
