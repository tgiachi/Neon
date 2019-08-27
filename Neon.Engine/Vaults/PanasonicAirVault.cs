using YamlDotNet.Serialization;

namespace Neon.Engine.Vaults
{
	public class PanasonicAirVault
	{
		[YamlMember(Alias = "token")]
		public string AccessToken { get; set; }

	}
}
