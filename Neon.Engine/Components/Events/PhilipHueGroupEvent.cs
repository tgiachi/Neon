using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("philips_hue_groups")]
	[IoTEntity("PHILIP_HUE_GROUP")]
	public class PhilipHueGroupEvent : NeonIoTBaseEntity
	{
		public string Model { get; set; }
		public int LightsCount { get; set; }
		public bool AnyOn { get; set; }

		public bool AllOn { get; set; }
	}
}
