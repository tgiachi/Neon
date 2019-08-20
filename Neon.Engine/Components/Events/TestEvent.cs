using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Interfaces.Entity;

namespace Neon.Engine.Components.Events
{
	public class TestEvent : INeonIoTEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string EntityType { get; set; }
		public DateTime EventDateTime { get; set; }

		public int Value { get; set; }
	}
}
