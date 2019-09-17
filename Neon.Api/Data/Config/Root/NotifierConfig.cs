using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Config.Common;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class NotifierConfig
	{

		[YamlMember(Alias = "notifier_config_directory")]
		public DirectoryConfig DirectoryConfig { get; set; }

		public NotifierConfig()
		{
			DirectoryConfig = new DirectoryConfig()
			{
				DirectoryName = "Notifiers"
			};
		}
	}
}
