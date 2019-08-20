using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class ComponentConfig
	{
		[YamlMember(Alias = "name")]
		public string Name { get; set; }
	}
}
