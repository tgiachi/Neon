using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class NeonConfig
	{
		[YamlMember(Alias = "engine")]
		public EngineConfig EngineConfig { get; set; }

		[YamlMember(Alias = "services")]
		public ServicesConfig ServicesConfig { get; set; }

		[YamlMember(Alias = "home")]
		public HomeConfig HomeConfig { get; set; }

		[YamlMember(Alias = "components")]
		public ComponentsConfig ComponentsConfig { get; set; }

		[YamlMember(Alias = "notifiers")]
		public NotifierConfig NotifierConfig { get; set; }

		public NeonConfig()
		{
			EngineConfig = new EngineConfig();
			ServicesConfig = new ServicesConfig();
			HomeConfig = new HomeConfig();
			ComponentsConfig = new ComponentsConfig();
			NotifierConfig = new NotifierConfig();
		}
	}
}
