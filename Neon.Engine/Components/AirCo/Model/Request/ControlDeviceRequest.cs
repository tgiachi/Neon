namespace Neon.Engine.Components.AirCo.Model.Request
{
	public class ControlDeviceRequest
	{
		public string DeviceGuid { get; set; }
		public ControlDeviceParameters Parameters { get; set; }
	}
}