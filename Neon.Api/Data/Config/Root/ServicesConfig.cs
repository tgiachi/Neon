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

		public ServicesConfig()
		{
			MqttConfig = new MqttConfig();
			IoTConfig = new IoTConfig();
			ScriptEngineConfig = new ScriptEngineConfig();
		}
	}
}
