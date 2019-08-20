﻿using Neon.Api.Data.Config.Common;
using Neon.Api.Utils;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Root
{
	public class EngineConfig
	{
		[YamlMember(Alias = "home_directory")]
		public string HomeDirectory { get; set; }

		[YamlMember(Alias = "use_swagger")]
		public bool UseSwagger { get; set; }

		[YamlMember(Alias = "logger")]
		public LoggerConfig Logger { get; set; }

		[YamlMember(Alias = "secret_key")]
		public string SecretKey { get; set; }

		public EngineConfig()
		{
			HomeDirectory = "./Neon";
			Logger = new LoggerConfig();
			UseSwagger = true;
			SecretKey = RandomStringUtils.RandomString(32);
		}
	}
}
