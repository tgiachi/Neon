using System;

namespace Neon.Api.Attributes.NoSql
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class NoSqlConnectorAttribute : Attribute
	{
		public string Name { get; set; }

		public NoSqlConnectorAttribute(string connectorName)
		{
			Name = connectorName;
		}

	}
}
