using YamlDotNet.Serialization;

namespace Neon.Api.Data.Config.Common
{
	public class DirectoryConfig
	{
		[YamlMember(Alias = "name")]
		public string DirectoryName { get; set; }
	}
}
