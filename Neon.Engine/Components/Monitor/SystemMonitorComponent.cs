using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Monitor
{
	[NeonComponent("system_monitor", "v1.0.0.0", "MONITOR", typeof(SystemMonitorConfig))]
	public class SystemMonitorComponent : AbstractNeonComponent<SystemMonitorConfig>
	{
		private readonly Process _process = Process.GetCurrentProcess();
		private DateTime _lastTimeStamp;
		private TimeSpan _lastTotalProcessorTime = TimeSpan.Zero;
		private TimeSpan _lastUserProcessorTime = TimeSpan.Zero;
		private TimeSpan _lastPrivilegedProcessorTime = TimeSpan.Zero;

		private int _count = 0;

		public SystemMonitorComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}

		[ComponentPollRate(30)]
		public override Task Poll()
		{
			CollectValues();
			return base.Poll();
		}

		private void CollectValues()
		{
			var entity = BuildEntity<SystemMonitorEvent>();
			var totalCpuTimeUsed = _process.TotalProcessorTime.TotalMilliseconds - _lastTotalProcessorTime.TotalMilliseconds;
			var privilegedCpuTimeUsed = _process.PrivilegedProcessorTime.TotalMilliseconds - _lastPrivilegedProcessorTime.TotalMilliseconds;
			var userCpuTimeUsed = _process.UserProcessorTime.TotalMilliseconds - _lastUserProcessorTime.TotalMilliseconds;

			_lastTotalProcessorTime = _process.TotalProcessorTime;
			_lastPrivilegedProcessorTime = _process.PrivilegedProcessorTime;
			_lastUserProcessorTime = _process.UserProcessorTime;

			var cpuTimeElapsed = (DateTime.UtcNow - _lastTimeStamp).TotalMilliseconds * Environment.ProcessorCount;
			_lastTimeStamp = DateTime.UtcNow;

			entity.TotalCpuUsed = totalCpuTimeUsed * 100 / cpuTimeElapsed;
			entity.PrivilegedCpuUsed = privilegedCpuTimeUsed * 100 / cpuTimeElapsed;
			entity.UserCpuUsed = userCpuTimeUsed * 100 / cpuTimeElapsed;

			entity.WorkingSet = _process.WorkingSet64;
			entity.NonPagedSystemMemory = _process.NonpagedSystemMemorySize64;
			entity.PagedMemory = _process.PagedMemorySize64;
			entity.PagedSystemMemory = _process.PagedSystemMemorySize64;
			entity.PrivateMemory = _process.PrivateMemorySize64;
			entity.VirtualMemory = _process.VirtualMemorySize64;

			if (_count > 0)
				PublishEntity(entity);

			_count++;
		}
	}
}
