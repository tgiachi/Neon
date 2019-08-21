using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;
using Newtonsoft.Json;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("owntracks")]
	[IoTEntity("OWNTRACKS_LOCATION")]
	public class OwnTracksEvent : NeonIoTBaseEntity
	{
		public string TrackerId { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public int Altitude { get; set; }

		public int BatteryLevel { get; set; }

		public int AccuracyMeters { get; set; }

		public float Pressure { get; set; }

		public double DistanceFromHomeInMeters{ get; set; }
	}
}
