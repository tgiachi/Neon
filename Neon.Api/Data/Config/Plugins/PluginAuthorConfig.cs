using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Plugins
{
	public class PluginAuthorConfig
	{
		[YamlMember(Alias = "author_name")]
		public string Name { get; set; } = string.Empty;

		[YamlMember(Alias = "author_email")]
		public string Email { get; set; } = string.Empty;


		public PluginAuthorConfig()
		{
			Name = "none";
			Email = "none@none";
		}

	}
}
