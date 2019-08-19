using System;

namespace Neon.Api.Attributes.Services
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public class NeonServiceAttribute : Attribute
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public int LoadOrder { get; set; } = 5;

		public NeonServiceAttribute(string name, string description, int order = 5)
		{
			Name = name;
			Description = description;
			LoadOrder = order;
		}

	}
}
