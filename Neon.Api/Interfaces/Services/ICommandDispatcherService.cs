using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Services
{
	public interface ICommandDispatcherService : INeonService
	{
		object Dispatch(string commandName, params object[] args);
	}
}
