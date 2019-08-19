using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Base
{
	public interface INeonService
	{
		Task<bool> Start();

		Task<bool> Stop();
	}
}
