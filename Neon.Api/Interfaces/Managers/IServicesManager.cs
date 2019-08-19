using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Managers
{
	public interface IServicesManager
	{
		Task<bool> Start();

		Task<bool> Stop();
	}
}
