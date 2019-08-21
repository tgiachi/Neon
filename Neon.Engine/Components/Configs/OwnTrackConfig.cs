using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Impl.Components;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs
{
	public class OwnTrackConfig : BaseComponentConfig
	{
		[YamlMember(Alias = "topic")]
		public string Topic { get; set; }


		public OwnTrackConfig()
		{
			Topic = "owntracks/+/+";
		}
	}
}
