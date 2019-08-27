using Neon.Api.Data.Config.Common;
using Neon.Api.Impl.Components;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs.Streaming
{
	public class RadarrConfig : BaseComponentConfig
	{
		[YamlMember(Alias = "api")]
		public ApiConfig Api { get; set; }

		[YamlMember(Alias = "server")]
		public HostConfig Server { get; set; }


		public RadarrConfig()
		{
			Api = new ApiConfig()
			{
				ApiKey = "change_me"
			};

			Server = new HostConfig()
			{
				Hostname = "change_me",
				Port = 0
			};
		}
	}
}
