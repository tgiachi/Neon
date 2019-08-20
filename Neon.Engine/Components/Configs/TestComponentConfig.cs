using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.SecretKey;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Components;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs
{
	public class TestComponentConfig : BaseComponentConfig
	{
		[SecretValue]
		[YamlMember(Alias = "test_value")]
		public string TestValue { get; set; }
	}
}
