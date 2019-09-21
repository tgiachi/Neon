using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Entities;

namespace Neon.Engine.Components.Events
{
	[EventsCollection("docker_containers")]
	[IoTEntity("DOCKER_CONTAINER")]
	public class DockerContainerEvent : NeonIoTBaseEntity
	{
		public string Status { get; set; }

		public string State { get; set; }

		public string Image { get; set; }

		public string ImageId { get; set; }

		public DateTime Created { get; set; }
	}
}
