using Newtonsoft.Json;

namespace Neon.Engine.Data
{
	public class SunSetResultData
	{
		[JsonProperty("sunrise")]
		public string Sunrise { get; set; }

		[JsonProperty("sunset")]
		public string Sunset { get; set; }

		[JsonProperty("day_length")]
		public string DayLength { get; set; }
	}
}
