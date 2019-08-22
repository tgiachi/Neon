using System;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Components
{
	public interface INeonComponent : IDisposable
	{
		string ComponentId { get; set; }

		Task<bool> Init(object config);

		Task<bool> Start();

		object GetDefaultConfig();

		Task Poll();

	}
}
