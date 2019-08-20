using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Data.Scheduler;
using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Services
{
	/// <summary>
	///     Interface for create Scheduler service
	/// </summary>
	public interface ISchedulerService : INeonService
	{

		/// <summary>
		///     Return all job information
		/// </summary>
		List<JobInfo> JobsInfo { get; set; }

		/// <summary>
		///     Add job with seconds
		/// </summary>
		/// <param name="job"></param>
		/// <param name="seconds"></param>
		/// <param name="startNow"></param>
		void AddJob(Action job, int seconds, bool startNow);

		void AddJob(Action job, string name, int seconds, bool startNow);

		void AddJob(Action job, string name, int hours, int minutes);

		void AddPolling(Action job, string name, SchedulerServicePollingEnum pollingType);
	}
}
