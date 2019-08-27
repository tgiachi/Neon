using Neon.Api.Interfaces.Components;
using YamlDotNet.Serialization;

namespace Neon.Api.Impl.Components
{
	public class BaseComponentConfig : INeonComponentConfig
	{
		[YamlMember(Alias = "polling_seconds")]
		public int PollingSeconds { get; set; }

		[YamlMember(Alias = "is_enabled")]
		public bool IsEnabled { get; set; }

		public BaseComponentConfig()
		{
			IsEnabled = true;
			PollingSeconds = -1;
		}
	}
}
