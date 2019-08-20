using System;

namespace Neon.Api.Attributes.Components
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ComponentPollRateAttribute : Attribute
	{
		public int Rate { get; set; }

		public bool IsEnabled { get; set; }

		public ComponentPollRateAttribute(int rate)
		{
			Rate = rate;

			IsEnabled = true;
		}

	}
}
