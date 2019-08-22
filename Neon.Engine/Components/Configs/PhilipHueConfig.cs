using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Impl.Components;
using Neon.Engine.Components.Configs.PhilipHue;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs
{
	public class PhilipHueConfig : BaseComponentConfig
	{
		[YamlMember(Alias = "use_remote")]
		public bool UseRemote { get; set; }

		
		[YamlMember(Alias = "remote")]
		public PhilipHueRemoteConfig RemoteConfig { get; set; }

		
		[YamlMember(Alias = "local")]
		public PhilipHueLocalConfig LocalConfig { get; set; }


		public PhilipHueConfig()
		{
			LocalConfig = new PhilipHueLocalConfig();
			RemoteConfig = new PhilipHueRemoteConfig();
			UseRemote = true;
		}
	}
}
