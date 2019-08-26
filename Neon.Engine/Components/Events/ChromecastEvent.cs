using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{

	[EventsCollection("chromecast")]
	[IoTEntity("CHROMECAST")]
	public class ChromecastEvent : NeonIoTBaseEntity
	{
		public string DeviceId { get; set; }

		public string IpAddress { get; set; }
	}
}
