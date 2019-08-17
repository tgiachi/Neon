using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class NeonConfig
	{
		[YamlMember(Alias = "engine")]
		public EngineConfig EngineConfig { get; set; }

		[YamlMember(Alias = "services")]
		public ServicesConfig ServicesConfig { get; set; }


		public NeonConfig()
		{
			EngineConfig = new EngineConfig();
			ServicesConfig = new ServicesConfig();
		}
	}
}
