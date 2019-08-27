using Neon.Api.Impl.Components;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs.AirConditioned
{
	public class PanasonicAirConfig : BaseComponentConfig
	{

		[YamlMember(Alias = "user_name")]
		public string Username { get; set; }


		[YamlMember(Alias = "password")]
		public string Password { get; set; }


		public PanasonicAirConfig()
		{
			Username = "change_me";
			Password = "change_me";
		}
	}
}
