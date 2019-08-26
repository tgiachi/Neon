using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("plex_hooks")]
	[IoTEntity("PLEX_HOOK", Name = "PLEX_HOOK")]
	public class PlexHookEvent : NeonIoTBaseEntity
	{
		public string Event { get; set; }

		public string LibrarySectionType { get; set; }

		public string LibrarySectionTitle { get; set; }

		public string RatingKey { get; set; }

		public string Key { get; set; }

		public string Guid { get; set; }

		public string Type { get; set; }

		public string Title { get; set; }

		public double Rating { get; set; }
	}
}
