using System;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Scheduler
{
	public interface IJobSchedulerTask : IDisposable
	{
		Task Execute(params object[] args);
	}
}
