using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Services;

namespace Neon.Engine.Std
{
	[ScriptModule]
	public class CommandsScriptModule
	{
		private ICommandDispatcherService _commandDispatcherService;

		public CommandsScriptModule(ICommandDispatcherService commandDispatcherService)
		{
			_commandDispatcherService = commandDispatcherService;
		}

		[ScriptFunction("send_command", "Send command")]
		public object SendCommand(string commandName, params object[] args)
		{
			return _commandDispatcherService.Dispatch(commandName, args);
		}
	}
}
