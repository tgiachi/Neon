using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class MqttConnectorConfig
	{

		[YamlMember(Alias = "hostname")]
		public string Hostname { get; set; }

		[YamlMember(Alias = "port")]
		public int Port { get; set; }

		public MqttConnectorConfig()
		{
			Hostname = "";
			Port = 0;
		}

	}
}
