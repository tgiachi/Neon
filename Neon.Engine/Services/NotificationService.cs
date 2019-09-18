using MediatR;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Neon.Api.Data;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.NoSql;
using Neon.Api.Interfaces.Notifiers;

namespace Neon.Engine.Services
{
	[NeonService("Notification Service", "Manager for notifications")]
	public class NotificationService : INotificationService
	{
		private readonly Dictionary<string, List<Action<object>>> _apiControllerListeners = new Dictionary<string, List<Action<object>>>();
		private readonly ILogger _logger;
		private readonly IMediator _mediator;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly INeonManager _neonManager;
		private readonly List<NotifierData> _notifierData;
		private readonly string _notifierConfigDirectory;
		private readonly Dictionary<string, INotifier> _runningNotifiers = new Dictionary<string, INotifier>();

		public NotificationService(ILogger<NotificationService> logger, IMediator mediator, IFileSystemManager fileSystemManager, INeonManager neonManager, List<NotifierData> notifierData, NeonConfig neonConfig)
		{
			_logger = logger;
			_fileSystemManager = fileSystemManager;
			_mediator = mediator;
			_neonManager = neonManager;
			_notifierData = notifierData;
			_notifierConfigDirectory = neonConfig.NotifierConfig.DirectoryConfig.DirectoryName;
		}

		public void NotifyConnector(string connectorName, string text, params object[] args)
		{
			if (_runningNotifiers.ContainsKey(connectorName))
			{
				_runningNotifiers[connectorName].Notify(text, args);
			}
		}

		public async void Broadcast<T>(object obj) where T : INotification
		{
			await _mediator.Publish(obj);
		}

		public async Task<TOut> RpcProcess<TIn, TOut>(TIn obj) where TIn : IRequest<TOut>
		{
			try
			{
				return await _mediator.Send(obj);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during rpc Process {ex.Message}");

				return default(TOut);
			}
		}

		private void EnsureComponentsDirectory()
		{
			_fileSystemManager.CreateDirectory(_notifierConfigDirectory);
		}

		public async Task<bool> Start()
		{
			EnsureComponentsDirectory();
			await StartNotifiers();
			return true;

		}

		private async Task StartNotifiers()
		{
			foreach (var notifierData in _notifierData)
			{
				await StartNotifier(notifierData);
			}

		}

		private async Task StartNotifier(NotifierData data)
		{
			try
			{
				_logger.LogDebug($"Starting Notifier {data.Name}");
				var notifier = _neonManager.Resolve(data.NotifierType) as INotifier;

				if (data.NotifierConfigType != null)
				{
					_logger.LogDebug($"Loading config for notifier {data.Name}");

					var cfg = _fileSystemManager.ReadFromFile(GetNotifierConfigFileName(data.Name), data.NotifierConfigType);

					if (cfg == null)
					{
						cfg = notifier.GetDefaultConfig();
						_fileSystemManager.WriteToFile(GetNotifierConfigFileName(data.Name), cfg);
					}

					await notifier.Init(cfg);
					_fileSystemManager.WriteToFile(GetNotifierConfigFileName(data.Name), cfg);
				}

				await notifier.Start();

				_runningNotifiers.Add(data.Name, notifier);

			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during starting notifier {data.Name} - {ex.Message}");
			}
		}

		private string GetNotifierConfigFileName(string notifierName)
		{
			return _fileSystemManager.BuildFilePath(Path.Combine(_notifierConfigDirectory, $"{notifierName}.yaml"));
		}

		public Task<bool> Stop()
		{
			foreach (var notifierValue in _runningNotifiers)
			{
				notifierValue.Value.Dispose();
			}

			return Task.FromResult(true);
		}
	}
}
