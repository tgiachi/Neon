using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Scheduler
{
	public interface IJobSchedulerTask : IDisposable
	{
		Task Execute(params object[] args);
	}
}
