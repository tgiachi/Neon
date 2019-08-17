using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces
{
	public interface IServicesManager
	{
		Task<bool> Start();

		Task<bool> Stop();
	}
}
