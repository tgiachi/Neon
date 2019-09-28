using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Services
{
	public interface ITaskQueueService : INeonService
	{
		int QueueCount { get; }

		int RunningTaskCount { get; }

		bool Queue(Func<Task> futureTask);


	}
}
