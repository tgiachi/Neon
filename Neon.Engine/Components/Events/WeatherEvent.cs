using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("weather")]
	[IoTEntity("WEATHER", Name = "WEATHER")]
	public class WeatherEvent : NeonIoTBaseEntity
	{
		public string Summary { get; set; }
		public double Temperature { get; set; }

		public double Humidity { get; set; }

		public double Pressure { get; set; }

		public double PrecipProbability { get; set; }

		public string Icon { get; set; }

	}
}
