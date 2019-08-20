using System;

namespace Neon.Api.Attributes.Components
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NeonComponentAttribute : Attribute
	{
		public string Name { get; set; }

		public string Version { get; set; }

		public string Category { get; set; }

		public string Description { get; set; }

		public string Author { get; set; }

		public Type ConfigType { get; set; }

		public NeonComponentAttribute(string name, string version, string category, Type configType)
		{
			Name = name;
			Version = version;
			Category = category;
			ConfigType = configType;
		}
	}
}
