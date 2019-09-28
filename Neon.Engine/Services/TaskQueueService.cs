using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.Services;

namespace Neon.Engine.Services
{

	[NeonService("Task Queue Service", "Execute queues")]
	public class TaskQueueService : ITaskQueueService
	{
		private readonly ConcurrentQueue<Func<Task>> _processingQueue = new ConcurrentQueue<Func<Task>>();
		private readonly ConcurrentDictionary<int, Task> _runningTasks = new ConcurrentDictionary<int, Task>();
		private int _maxParallelizationCount;
		private int _maxQueueLength;
		private TaskCompletionSource<bool> _tscQueue = new TaskCompletionSource<bool>();

		public int QueueCount => _processingQueue.Count;
		public int RunningTaskCount => _runningTasks.Count;

		public Task<bool> Start()
		{
			_maxParallelizationCount = 5;
			_maxQueueLength = int.MaxValue / 2;

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);

		}

		public bool Queue(Func<Task> futureTask)
		{
			if (_processingQueue.Count < _maxQueueLength)
			{
				_processingQueue.Enqueue(futureTask);
				return true;
			}
			return false;
		}

		public async Task Process()
		{
			var t = _tscQueue.Task;
			StartTasks();
			await t;
		}

		public void ProcessBackground(Action<Exception> exception = null)
		{
			Task.Run(Process).ContinueWith(t =>
			{
				exception?.Invoke(t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		private void StartTasks()
		{
			var startMaxCount = _maxParallelizationCount - _runningTasks.Count;
			for (int i = 0; i < startMaxCount; i++)
			{
				Func<Task> futureTask;
				if (!_processingQueue.TryDequeue(out futureTask))
				{
					// Queue is most likely empty
					break;
				}

				var t = Task.Run(futureTask);
				if (!_runningTasks.TryAdd(t.GetHashCode(), t))
				{
					throw new Exception("Should not happen, hash codes are unique");
				}

				t.ContinueWith((t2) =>
				{
					Task _temp;
					if (!_runningTasks.TryRemove(t2.GetHashCode(), out _temp))
					{
						throw new Exception("Should not happen, hash codes are unique");
					}

					// Continue the queue processing
					StartTasks();
				});
			}

			if (_processingQueue.IsEmpty && _runningTasks.IsEmpty)
			{
				// Interlocked.Exchange might not be necessary
				var _oldQueue = Interlocked.Exchange(
					ref _tscQueue, new TaskCompletionSource<bool>());
				_oldQueue.TrySetResult(true);
			}
		}
	}

}
