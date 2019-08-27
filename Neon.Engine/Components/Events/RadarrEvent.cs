using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{

	[EventsCollection("radarr")]
	[IoTEntity("RADARR", Name = "RADARR")]
	public class RadarrEvent : NeonIoTBaseEntity
	{
		public int MoviesCount { get; set; }
	}
}
