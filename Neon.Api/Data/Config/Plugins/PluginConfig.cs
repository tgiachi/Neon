using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Plugins
{
	public class PluginConfig
	{
		[YamlMember(Alias = "author")]
		public PluginAuthorConfig PluginAuthorConfig { get; set; }

		[YamlMember(Alias = "info")]
		public PluginInfoConfig PluginInfoConfig { get; set; }

		[YamlMember(Alias = "plugin_filename")]
		public string PluginDllName { get; set; }

		public PluginConfig()
		{
			PluginAuthorConfig = new PluginAuthorConfig();
			PluginInfoConfig = new PluginInfoConfig();
			PluginDllName = string.Empty;
		}
	}
}
