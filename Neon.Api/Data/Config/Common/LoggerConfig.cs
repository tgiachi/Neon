using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class LoggerConfig
	{
		[YamlMember(Alias = "log_directory")]
		public string LogDirectory { get; set; }


		[YamlMember(Alias = "log_level")]
		public LogLevelEnum Level { get; set; }


		public LoggerConfig()
		{
			Level = LogLevelEnum.Debug;
			LogDirectory = "Logs";
		}
	}

	public enum LogLevelEnum
	{
		Debug,
		Info,
		Warning,
		Error
	}
}
