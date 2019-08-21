using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.Services;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("User interaction service", "Manager user interaction")]
	public class UserInteractionService : IUserInteractionService
	{
		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}
