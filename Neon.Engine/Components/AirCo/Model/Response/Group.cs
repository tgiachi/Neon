using System.Collections.Generic;

namespace Neon.Engine.Components.AirCo.Model.Response
{
	public class Group
	{
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		public List<DeviceId> DeviceIdList { get; set; }
	}
}