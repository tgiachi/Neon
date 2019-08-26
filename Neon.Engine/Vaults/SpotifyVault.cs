using Neon.Api.Data.Config.Common;
using YamlDotNet.Serialization;

namespace Neon.Engine.Vaults
{
	public class SpotifyVault
	{
		[YamlMember(Alias = "access_token")]
		public AccessTokenConfig AccessToken { get; set; }


		public SpotifyVault()
		{
			AccessToken = new AccessTokenConfig();
		}
	}
}
