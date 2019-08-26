using MediatR;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("Notification Service", "Manager for notifications")]
	public class NotificationService : INotificationService
	{
		private readonly Dictionary<string, List<Action<object>>> _apiControllerListeners = new Dictionary<string, List<Action<object>>>();
		private readonly ILogger _logger;
		private readonly IMediator _mediator;

		public NotificationService(ILogger<NotificationService> logger, IMediator mediator)
		{
			_logger = logger;
			_mediator = mediator;
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

		public Task<bool> Start()
		{
			return Task.FromResult(true);

		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}
