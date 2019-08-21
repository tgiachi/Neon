using Neon.Api.Attributes.Components;
using System.Reflection;

namespace Neon.Api.Data.Components
{
	public class ComponentPollMethodData
	{
		public MethodInfo Method { get; set; }

		public ComponentPollRateAttribute ComponentPollRate { get; set; }
	}
}
