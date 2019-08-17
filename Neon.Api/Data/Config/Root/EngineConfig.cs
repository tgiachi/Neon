using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class EngineConfig
	{
		[YamlMember(Alias = "home_directory")]
		public string HomeDirectory { get; set; }

		[YamlMember(Alias = "logs_directory")]
		public string LogsDirectory { get; set; }

		public EngineConfig()
		{
			HomeDirectory = "./Neon";
			LogsDirectory = "logs";
		}
	}
}
