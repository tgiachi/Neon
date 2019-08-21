using Neon.Api.Data.Config.Common;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class ComponentsConfig
	{
		[YamlMember(Alias = "to_load")]
		public List<ComponentConfig> ComponentsToLoad { get; set; }

		[YamlMember(Alias = "config_directory")]
		public DirectoryConfig ConfigDirectory { get; set; }

		public ComponentsConfig()
		{
			ComponentsToLoad = new List<ComponentConfig>();
			ConfigDirectory = new DirectoryConfig()
			{
				DirectoryName = "Components"
			};
		}
	}
}
