using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Entities;
using Neon.Api.Interfaces.Entity;

namespace Neon.Engine.Components.Events
{
	public class TestEvent : NeonIoTBaseEntity
	{
		public int Value { get; set; }
	}
}
