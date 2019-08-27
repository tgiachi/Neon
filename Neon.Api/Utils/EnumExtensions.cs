using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Scheduler;

namespace Neon.Api.Utils
{
	public static class EnumExtensions
	{

		public static int Value(this SchedulerServicePollingEnum value)
		{
			return (int) value;
		}
	
	}
}
