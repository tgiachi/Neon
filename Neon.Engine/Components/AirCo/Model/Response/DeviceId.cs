﻿namespace Neon.Engine.Components.AirCo.Model.Response
{
	public class DeviceId
	{
		public string DeviceGuid { get; set; }
		public string DeviceType { get; set; }
		public string DeviceName { get; set; }
		public int Permission { get; set; }
		public int SummerHouse { get; set; }
		public bool IAutoX { get; set; }
		public bool Nanoe { get; set; }
		public bool AutoMode { get; set; }
		public bool HeatMode { get; set; }
		public bool FanMode { get; set; }
		public bool DryMode { get; set; }
		public bool CoolMode { get; set; }
		public bool EcoNavi { get; set; }
		public bool PowerfulMode { get; set; }
		public bool QuietMode { get; set; }
		public bool AirSwingLR { get; set; }
		public DeviceParameters Parameters { get; set; }
	}
}