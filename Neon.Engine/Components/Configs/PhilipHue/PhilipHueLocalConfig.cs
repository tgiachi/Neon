using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs.PhilipHue
{
	public class PhilipHueLocalConfig
	{
		[YamlMember(Alias = "bridge_ip_address")]
		public string BridgeIpAddress { get; set; }

		public PhilipHueLocalConfig()
		{
			BridgeIpAddress = "";
		}
	}
}
