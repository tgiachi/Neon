using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{

	[EventsCollection("sonarr")]
	[IoTEntity("SONARR", Name = "SONARR")]
	public class SonarrEvent : NeonIoTBaseEntity
	{
		public int SeriesCount { get; set; }
	}
}
