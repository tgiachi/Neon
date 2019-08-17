﻿using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class MqttConfig
	{
		[YamlMember(Alias = "client")]
		public MqttConnectorConfig Client { get; set; }

		[YamlMember(Alias = "use_embedded_server")]
		public bool UseEmbeddedServer { get; set; }

		[YamlMember(Alias = "embedded_server_port")]
		public int EmbeddedServerPort { get; set; }

		public MqttConfig()
		{
			Client = new MqttConnectorConfig();
			UseEmbeddedServer = false;
			EmbeddedServerPort = 1883;
		}
		
	}
}
