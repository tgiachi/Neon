using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Commands;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{

	[NeonService("Command dispatcher", "Send/Receive commands from components/script engine", 8)]
	public class CommandDispatcherService : ICommandDispatcherService
	{
		private readonly ILogger _logger;
		public List<CommandPreloadData> CommandPreload { get; }
		private readonly INeonManager _neonManager;

		private Dictionary<string, CommandPreloadData> _commands = new Dictionary<string, CommandPreloadData>();

		public CommandDispatcherService(ILogger<CommandDispatcherService> logger,
			List<CommandPreloadData> commandPreloadData, INeonManager neonManager)
		{
			_logger = logger;
			_neonManager = neonManager;
			CommandPreload = commandPreloadData;
			CommandPreload.ForEach(c =>
			{
				_commands.Add(c.CommandName, c);
			});
		}

		public Task<bool> Start()
		{
			return Task.FromResult(true);

		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}



		public object Dispatch(string commandName, params object[] args)
		{
			if (_commands.ContainsKey(commandName))
			{
				var commandObj = _commands[commandName];
				var objToCall = _neonManager.Resolve(commandObj.SourceType);

				_logger.LogDebug($"Executing command {commandName} [is Async = {commandObj.IsAsync}]");

				if (commandObj.IsAsync)
				{
					var task = (Task)commandObj.Method.Invoke(objToCall, args);
					task.ConfigureAwait(false); ;

					return (object)((dynamic)task).Result;
				}

				return commandObj.Method.Invoke(objToCall, args);
			}

			throw new Exception($"Command {commandName} not found!");
		}
	}
}
