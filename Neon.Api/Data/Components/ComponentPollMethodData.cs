using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Neon.Api.Attributes.Components;

namespace Neon.Api.Data.Components
{
	public class ComponentPollMethodData
	{
		public MethodInfo Method { get; set; }

		public ComponentPollRateAttribute ComponentPollRate { get; set; }
	}
}
