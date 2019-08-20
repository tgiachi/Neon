using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.Scheduler
{
	/// <summary>
	///     Enum for choose polling timer
	/// </summary>
	public enum SchedulerServicePollingEnum
	{
		ShortPolling = 10,
		NormalPolling = 60,
		LongPolling = 300,
		VeryLongPolling = 3600
	}
}
