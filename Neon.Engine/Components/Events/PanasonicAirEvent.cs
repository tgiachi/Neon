using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;
using Neon.Engine.Components.AirCo.Model;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("air_conditioned")]
	[IoTEntity("AIR_CONDITIONED")]
	public class PanasonicAirEvent : NeonIoTBaseEntity
	{
		public string AirCondGroup { get; set; }

		public string DeviceGuid { get; set; }

		public string DeviceType { get; set; }

		public bool IsOn { get; set; }

		public PanasonicAirModeEnum DeviceMode { get; set; }

		public FanSpeedType FanSpeed { get; set; }
		public decimal Temperature { get; set; }
		public AirSwingUDType AirSwingUD { get; set; }
		public AirswingLRType AirSwingLR { get; set; }
	}

	public enum PanasonicAirModeEnum
	{
		Dry,
		Auto,
		Heat,
		Cool,
		Fan,
		None
	}
}
