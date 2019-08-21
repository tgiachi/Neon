using Newtonsoft.Json;

namespace Neon.Engine.Data
{
	public class OwnTracksData
	{
		[JsonProperty("lat")]
		public double Latitude { get; set; }

		[JsonProperty("lon")]
		public double Longitude { get; set; }

		[JsonProperty("alt")]
		public int Altitude { get; set; }

		[JsonProperty("batt")]
		public int BatteryLevel { get; set; }

		[JsonProperty("acc")]
		public int AccuracyMeters { get; set; }

		[JsonProperty("p")]
		public float Pressure { get; set; }

		[JsonProperty("tid")]
		public string Id { get; set; }
	}
}
