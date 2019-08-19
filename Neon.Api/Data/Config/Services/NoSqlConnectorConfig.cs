using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class NoSqlConnectorConfig
	{
		[YamlMember(Alias = "name")]
		public string Name { get; set; }

		[YamlMember(Alias = "connection_string")]
		public string ConnectionString { get; set; }

	}
}
