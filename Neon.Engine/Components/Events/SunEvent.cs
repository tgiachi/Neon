using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;
using System;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("SUNSET_RISE")]
	[IoTEntity("SUN", Name = "SUN")]
	public class SunEvent : NeonIoTBaseEntity
	{
		public DateTime Date { get; set; }
		public DateTime SunSet { get; set; }
		public DateTime SunRise { get; set; }
		public TimeSpan DayLength { get; set; }

	}
}
