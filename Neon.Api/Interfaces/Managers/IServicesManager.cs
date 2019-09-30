using System.Threading.Tasks;
using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Managers
{
	public interface IServicesManager
	{
		Task<bool> Start();

		Task<bool> Stop();

		T GetService<T>() where T : INeonService;
	}
}
