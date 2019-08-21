using Neon.Api.Interfaces.Components;
using YamlDotNet.Serialization;

namespace Neon.Api.Impl.Components
{
	public class BaseComponentConfig : INeonComponentConfig
	{
		[YamlMember(Alias = "is_enabled")]
		public bool IsEnabled { get; set; }
	}
}
