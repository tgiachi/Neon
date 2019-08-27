using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class HostConfig
	{
		[YamlMember(Alias = "hostname")]
		public string Hostname { get; set; }

		[YamlMember(Alias = "port")]
		public int Port { get; set; }
	}
}
