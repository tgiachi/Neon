using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class CoordinateConfig
	{

		[YamlMember(Alias = "latitude")]
		public double Latitude { get; set; }

		[YamlMember(Alias = "longitude")]
		public double Longitude { get; set; }

		[YamlMember(Alias = "elevation")]
		public double Elevation { get; set; }

		public CoordinateConfig()
		{
			Latitude = 0.1;
			Longitude = 0.1;
			Elevation = 1;
		}
	}
}
