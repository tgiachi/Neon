using Neon.Api.Data.Config.Common;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Services
{
	public class ScriptEngineConfig
	{
		[YamlMember(Alias = "scripts_directory")]
		public DirectoryConfig ScriptsDirectory { get; set; }

		[YamlMember(Alias = "modules_directory")]
		public DirectoryConfig ModulesDirectory { get; set; }

		[YamlMember(Alias = "write_output_on_log")]
		public bool WriteOnLogOutput { get; set; }

		[YamlMember(Alias = "write_output_on_console")]
		public bool WriteOnConsoleOutput { get; set; }


		public ScriptEngineConfig()
		{
			ScriptsDirectory = new DirectoryConfig()
			{
				DirectoryName = "Scripts"
			};

			ModulesDirectory = new DirectoryConfig()
			{
				DirectoryName = "Modules"
			};

			WriteOnConsoleOutput = false;
			WriteOnLogOutput = true;

		}
	}
}
