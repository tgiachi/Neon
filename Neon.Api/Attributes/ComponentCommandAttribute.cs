using System;

namespace Neon.Api.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ComponentCommandAttribute : Attribute
	{
		public string Name { get; set; }

		public string HelpText { get; set; }

		public ComponentCommandAttribute(string name, string helpText)
		{
			Name = name;
			HelpText = helpText;
		}
	}
}
