using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("broadlink_devices")]
	[IoTEntity("BROADLINK_DEVICE")]
	public class BroadlinkDeviceEvent : NeonIoTBaseEntity
	{
		public string MacAddress { get; set; }

		public int DeviceType { get; set; }
	}
}
