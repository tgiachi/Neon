using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Data.ScriptEngine;
using Neon.Api.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.Std
{

	[ScriptModule]
	public class SystemScriptModule
	{
		private readonly IScriptEngineService _scriptEngineService;

		public SystemScriptModule(IScriptEngineService scriptEngineService)
		{
			_scriptEngineService = scriptEngineService;
		}

		[ScriptFunction("help", "Describe function")]
		public ScriptFunctionData HelpFunction(string functionName)
		{
			return _scriptEngineService.Functions.FirstOrDefault(f => f.FunctionName.ToLower() == functionName.ToLower());
		}

		[ScriptFunction("execute_task", "Execute task with callback")]
		public void ExecuteTask(string taskName, Delegate function, Delegate callback)
		{
			Task.Factory.StartNew(() => function.DynamicInvoke()).ContinueWith(task => callback.DynamicInvoke(taskName));
		}
	}
}
