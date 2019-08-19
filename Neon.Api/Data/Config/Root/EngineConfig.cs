using Neon.Api.Utils;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class EngineConfig
	{
		[YamlMember(Alias = "home_directory")]
		public string HomeDirectory { get; set; }


		[YamlMember(Alias = "use_swagger")]
		public bool UseSwagger { get; set; }

		[YamlMember(Alias = "logs_directory")]
		public string LogsDirectory { get; set; }

		[YamlMember(Alias = "secret_key")]
		public string SecretKey { get; set; }

		public EngineConfig()
		{
			HomeDirectory = "./Neon";
			LogsDirectory = "logs";
			UseSwagger = true;
			SecretKey = RandomStringUtils.RandomString(30);
		}
	}
}
