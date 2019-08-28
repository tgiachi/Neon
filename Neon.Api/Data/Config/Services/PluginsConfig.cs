using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Config.Common;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class PluginsConfig
	{
		[YamlMember(Alias = "directory")]
		public DirectoryConfig Directory { get; set; }


		public PluginsConfig()
		{
			Directory = new DirectoryConfig()
			{
				DirectoryName = "Plugins"
			};
		}
	}
}
