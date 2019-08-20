using System;

namespace Neon.Api.Attributes.Config
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigEnvVariableAttribute : Attribute
	{
		public string EnvName { get; set; }

		public ConfigEnvVariableAttribute(string envName)
		{
			EnvName = envName;
		}
	}
}
