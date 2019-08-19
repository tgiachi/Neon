using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.Mqtt
{
	public class MqttMessage
	{
		public string Topic { get; set; }

		public string Payload { get; set; }
	}
}
