using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Config.Common;
using Neon.Api.Impl.Components;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs.Music
{
	public class SpotifyConfig : BaseComponentConfig
	{
		[YamlMember(Alias = "api")]
		public ClientSecretConfig ClientSecret { get; set; }


		public SpotifyConfig()
		{
			ClientSecret = new ClientSecretConfig()
			{
				ClientId = "change_me"
			};
		}
	}
}
