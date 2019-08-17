using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Services
{
	public interface INeonService
	{
		Task<bool> Start();

		Task<bool> Stop();
	}
}
