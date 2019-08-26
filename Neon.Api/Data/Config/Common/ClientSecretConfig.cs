using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class ClientSecretConfig
	{

		[YamlMember(Alias = "client_id")]
		public string ClientId { get; set; }

		[YamlMember(Alias = "client_secret")]
		public string ClientSecret { get; set; }

	}
}
