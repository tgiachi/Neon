using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class ApiConfig
	{
		[YamlMember(Alias = "api_key")]
		public string ApiKey { get; set; }
	}
}
