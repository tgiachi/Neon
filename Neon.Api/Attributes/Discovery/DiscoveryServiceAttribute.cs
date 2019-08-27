using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Attributes.Discovery
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class DiscoveryServiceAttribute : Attribute
	{
		public string ServiceName { get; set; }

		public DiscoveryServiceAttribute(string serviceName)
		{
			ServiceName = serviceName;

		}
	}
}
