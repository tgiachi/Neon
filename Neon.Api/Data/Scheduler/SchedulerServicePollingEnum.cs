﻿namespace Neon.Api.Data.Scheduler
{
	/// <summary>
	///     Enum for choose polling timer
	/// </summary>
	public enum SchedulerServicePollingEnum
	{
		EverySecondPolling = 1,
		ShortPolling = 10,
		HalfNormalPolling = 60,
		NormalPolling = 60,
		LongPolling = 300,
		VeryLongPolling = 3600
	}

}
