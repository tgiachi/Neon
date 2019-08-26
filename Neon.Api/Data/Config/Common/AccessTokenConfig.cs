using System;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class AccessTokenConfig
	{
		[YamlMember(Alias = "token_type")]
		public string TokenType { get; set; }

		[YamlMember(Alias = "access_token")]
		public string AccessToken { get; set; }

		[YamlMember(Alias = "refresh_token")]
		public string RefreshToken { get; set; }

		[YamlMember(Alias = "expire_on")]
		public DateTime ExpireOn { get; set; }
	}
}
