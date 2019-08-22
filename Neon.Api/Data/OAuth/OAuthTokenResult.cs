using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Neon.Api.Data.OAuth
{
	public class OAuthTokenResult
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_Type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }
		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }
	}
}
