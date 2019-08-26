using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("philips_hue_lights")]
	[IoTEntity("PHILIP_HUE_LIGHT")]
	public class PhilipHueLightEvent : NeonIoTBaseEntity
	{
		public string LightId { get; set; }
		public bool IsOn { get; set; }

		public string ColorMode { get; set; }

		public string LightType { get; set; }

		public string LightGroup { get; set; }

		public double[] ColorCoordinates { get; set; }
	}
}
