using System;

namespace Neon.Api.Attributes.Entities
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class IoTEntityAttribute : Attribute
	{
		public string Group { get; set; }

		public string Name { get; set; }


		public IoTEntityAttribute(string group)
		{
			Group = group;
		}
	}
}
