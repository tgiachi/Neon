using System.IO;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class IoTConfig
	{
		[YamlMember(Alias = "entities_database")]
		public NoSqlConnectorConfig EntitiesDb { get; set; }

		[YamlMember(Alias = "events_database")]
		public NoSqlConnectorConfig EventsDb { get; set; }


		public IoTConfig()
		{
			EntitiesDb = new NoSqlConnectorConfig()
			{
				Name = "lite_db",
				ConnectionString = Path.Combine("db", "entities.db")
			};

			EventsDb = new NoSqlConnectorConfig()
			{
				Name = "lite_db",
				ConnectionString = Path.Combine("db", "events.db")
			};
		}
	}
}
