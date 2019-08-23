using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Services;

namespace Neon.Engine.Std
{
	[ScriptModule]
	public class StatesScriptModule
	{
		private readonly IStateService _stateService;

		public StatesScriptModule(IStateService stateService)
		{
			_stateService = stateService;
		}

		[ScriptFunction("set_state", "Set state")]
		public void SetState(string name, object value)
		{
			_stateService.SetState(name, value);
		}

		[ScriptFunction("increment_state", "Increment state")]
		public bool IncrementState(string name, int count = 1)
		{
			return _stateService.IncrementState(name, count);
		}

		[ScriptFunction("decrement_state", "Decrement state")]
		public bool DecrementState(string name, int count = 1)
		{
			return _stateService.DecrementState(name, count);
		}

		[ScriptFunction("get_state", "Get state")]
		public object GetState(string name)
		{
			return _stateService.GetState(name);
		}

		[ScriptFunction("get_boolean_state", "Get Boolean state")]
		public bool GetBooleanState(string name)
		{
			return _stateService.GetBooleanState(name);
		}

		[ScriptFunction("set_boolean_state", "Set boolean state")]
		public void SetBooleanState(string name, bool value)
		{
			_stateService.SetBooleanState(name, value);
		}
	}
}
