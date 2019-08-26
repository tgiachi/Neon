using Q42.HueApi.Models;
using YamlDotNet.Serialization;

namespace Neon.Engine.Vaults
{
	public class PhilipHueVault
	{
		[YamlMember(Alias = "token")]
		public AccessTokenResponse AccessToken { get; set; }

		[YamlMember(Alias = "local_api_key")]
		public string LocalApiKey { get; set; }


	}
}
