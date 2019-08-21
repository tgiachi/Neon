using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;
using System.Collections.Generic;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("sonoff_devices")]

	[IoTEntity("SONOFF_DEVICES")]
	public class SonoffTasmodaEvent : NeonIoTBaseEntity
	{
		public string Status { get; set; }

		public int PowerCount { get; set; }

		public List<SonoffTasmodaPowerStatus> PowerStatuses { get; set; }

		public SonoffTasmodaEvent()
		{
			PowerStatuses = new List<SonoffTasmodaPowerStatus>();
		}
	}
}
