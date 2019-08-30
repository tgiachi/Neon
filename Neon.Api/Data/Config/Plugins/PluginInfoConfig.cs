using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Plugins
{
	public class PluginInfoConfig
	{
		[YamlMember(Alias = "plugin_name")]
		public string PluginName { get; set; } = string.Empty;

		[YamlMember(Alias = "plugin_version")]
		public string PluginVersion { get; set; } = string.Empty;

		[YamlMember(Alias = "description")]
		public string PluginDescription { get; set; } = string.Empty;

	}
}
