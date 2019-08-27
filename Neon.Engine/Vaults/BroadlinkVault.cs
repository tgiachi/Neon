using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Neon.Engine.Vaults
{
	public class BroadlinkVault
	{
		[YamlMember(Alias = "devices")]
		public List<BroadlinkDeviceData> Devices { get; set; }

		public BroadlinkVault()
		{
			Devices = new List<BroadlinkDeviceData>();
		}
	}



	public class BroadlinkDeviceData
	{

		[YamlMember(Alias = "device_id")]
		public string DeviceId { get; set; }

		[YamlMember(Alias = "encrypt_key")]
		public string EncryptKey { get; set; }

		[YamlMember(Alias = "mac_address")]
		public string MacAddress { get; set; }

		[YamlMember(Alias = "ip_address")]
		public string IpAddress { get; set; }
	}
}
