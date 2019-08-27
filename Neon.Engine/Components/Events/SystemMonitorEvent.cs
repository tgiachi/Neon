using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("system_monitor")]
	[IoTEntity("SYSTEM_MONITOR", Name = "SYSTEM")]
	public class SystemMonitorEvent : NeonIoTBaseEntity
	{
		public double TotalCpuUsed { get; set; }

		public double PrivilegedCpuUsed { get; set; }

		public double UserCpuUsed { get; set; }

		public long WorkingSet { get; set; }

		public long NonPagedSystemMemory { get; set; }

		public long PagedMemory { get; set; }
		public long PagedSystemMemory { get; set; }

		public long PrivateMemory { get; set; }

		public long VirtualMemory { get; set; }

	}
}
