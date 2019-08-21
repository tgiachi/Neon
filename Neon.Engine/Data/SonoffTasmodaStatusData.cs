using Newtonsoft.Json;

namespace Neon.Engine.Data
{
	public class SonoffTasmodaStatusData
	{
		public string Time { get; set; }

		public string Uptime { get; set; }

		public int Heap { get; set; }

		public string SleepMode { get; set; }

		public int Sleep { get; set; }

		public int LoadAvg { get; set; }

		[JsonProperty("POWER1")]
		public string Power1 { get; set; }

		[JsonProperty("POWER2")]
		public string Power2 { get; set; }

		[JsonProperty("POWER3")]
		public string Power3 { get; set; }

		[JsonProperty("POWER4")]
		public string Power4 { get; set; }

	}
}
