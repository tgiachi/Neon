using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs.PhilipHue
{
	public class PhilipHueRemoteConfig
	{
		[YamlMember(Alias = "app_id")]
		public string AppId { get; set; }

		[YamlMember(Alias = "client_id")]
		public string ClientId { get; set; }

		[YamlMember(Alias = "client_secret")]
		public string ClientSecret { get; set; }


		public PhilipHueRemoteConfig()
		{
			AppId = "change_me";
			ClientId = "change_me";
			ClientSecret = "change_me";
		}
	}
}
