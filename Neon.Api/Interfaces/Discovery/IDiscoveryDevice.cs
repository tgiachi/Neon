using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Discovery;

namespace Neon.Api.Interfaces.Discovery
{
	public interface IDiscoveryDevice
	{ 
		void OnDeviceDiscovered(DiscoveryDevice device);
	}
}
