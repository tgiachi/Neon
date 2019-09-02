using Neon.Api.Data.Commands;
using Neon.Api.Interfaces.Base;
using System.Collections.Generic;

namespace Neon.Api.Interfaces.Services
{
	public interface ICommandDispatcherService : INeonService
	{
		List<CommandPreloadData> CommandPreload { get; }
		object Dispatch(string commandName, params object[] args);
	}
}
