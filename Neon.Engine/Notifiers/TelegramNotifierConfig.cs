using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Engine.Notifiers
{
	public class TelegramNotifierConfig
	{
		[YamlMember(Alias = "api_key")]
		public string ApiKey { get; set; }

		[YamlMember(Alias = "persistence_connector")]
		public string PersistenceConnector { get; set; }


		public TelegramNotifierConfig()
		{
			ApiKey = "change_me";
			PersistenceConnector = "flat_json_db";
		}
	}
}
