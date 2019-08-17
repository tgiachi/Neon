using Neon.Api.Data.Config.Services;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class ServicesConfig
	{
		[YamlMember(Alias = "mqtt")]
		public MqttConfig MqttConfig { get; set; }


		public ServicesConfig()
		{
			MqttConfig = new MqttConfig();
		}
	}
}
