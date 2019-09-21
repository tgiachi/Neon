using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Impl.Components;

namespace Neon.Engine.Components.Configs.System
{
	public class DockerConfig : BaseComponentConfig
	{
		public string ServerUrl { get; set; }

		public DockerConfig()
		{
			ServerUrl = "change_me";
		}
	}
}
