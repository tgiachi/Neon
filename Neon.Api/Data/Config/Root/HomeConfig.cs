using Neon.Api.Data.Config.Common;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class HomeConfig
	{
		[YamlMember(Alias = "name")]
		public string Name { get; set; }

		[YamlMember(Alias = "coordinate")]
		public CoordinateConfig CoordinateConfig { get; set; }

		public HomeConfig()
		{
			CoordinateConfig = new CoordinateConfig();
			Name = "My Home";
		}
	}
}
