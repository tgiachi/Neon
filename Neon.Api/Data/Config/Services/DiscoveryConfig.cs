using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class DiscoveryConfig
	{
		[YamlMember(Alias = "enabled")]
		public bool EnabledDiscovery { get;set; }

		public DiscoveryConfig()
		{
			EnabledDiscovery = true;
		}
	}
}
