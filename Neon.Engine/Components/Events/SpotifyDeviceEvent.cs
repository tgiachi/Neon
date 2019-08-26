using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("spotify_devices")]
	[IoTEntity("DEVICES")]
	public class SpotifyDeviceEvent : NeonIoTBaseEntity
	{
		public string DeviceId { get; set; }

		public int VolumePercent { get; set; }

		public string DeviceType { get; set; }

		public bool IsActive { get; set; }

		public bool IsRestricted { get; set; }
	}
}
