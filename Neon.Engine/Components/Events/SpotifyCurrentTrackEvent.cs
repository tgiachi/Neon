using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("spotify_current_tracks")]
	[IoTEntity("CURRENT_TRACKS", Name = "SPOTIFY_CURRENT_PLAYING")]
	public class SpotifyCurrentTrackEvent : NeonIoTBaseEntity
	{
		public string ArtistName { get; set; }

		public string SongName { get; set; }

		public string Uri { get; set; }

		public bool IsPlaying { get; set; }
	}
}
