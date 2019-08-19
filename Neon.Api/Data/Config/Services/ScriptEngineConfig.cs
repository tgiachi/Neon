using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Config.Common;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class ScriptEngineConfig
	{
		[YamlMember(Alias = "directory")]
		public DirectoryConfig Directory { get; set; }

		public ScriptEngineConfig()
		{
			Directory = new DirectoryConfig()
			{
				DirectoryName = "Scripts"
			};
		}
	}
}
