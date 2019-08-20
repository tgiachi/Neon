﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.Scheduler
{
	public class JobInfo
	{
		public Guid JobId { get; set; }
		public JobTypeEnum JobType { get; set; }
		public string JobName { get; set; }
		public int Seconds { get; set; }
		public bool StartNow { get; set; }

		public DateTime LastExecution { get; set; }

		public DateTime NextExecution { get; set; }
	}
}
