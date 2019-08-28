using Neon.Api.Data.Config.Services;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class ServicesConfig
	{
		[YamlMember(Alias = "mqtt")]
		public MqttConfig MqttConfig { get; set; }

		[YamlMember(Alias = "script_engine")]
		public ScriptEngineConfig ScriptEngineConfig { get; set; }

		[YamlMember(Alias = "iot")]
		public IoTConfig IoTConfig { get; set; }

		[YamlMember(Alias = "discovery")]
		public DiscoveryConfig DiscoveryConfig { get; set; }


		[YamlMember(Alias = "plugins")]
		public PluginsConfig PluginsConfig { get; set; }

		public ServicesConfig()
		{
			MqttConfig = new MqttConfig();
			IoTConfig = new IoTConfig();
			DiscoveryConfig = new DiscoveryConfig();
			ScriptEngineConfig = new ScriptEngineConfig();
			PluginsConfig = new PluginsConfig();
		}
	}
}
