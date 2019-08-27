using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.Discovery
{
	public class DiscoveryDevice
	{
		public string Name { get; set; }
		public string Service { get; set; }

		public string IpAddress { get; set; }

		public int Port { get; set; }

		public int Ttl { get; set; }

		public Dictionary<string, string> Properties { get; set; }

		public DiscoveryDevice()
		{
			Properties = new Dictionary<string, string>();
		}

		public override string ToString()
		{
			return $"{Service} - {IpAddress}:{Port}";
		}
	}
}
