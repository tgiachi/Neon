using System;
using Neon.Api.Attributes.Websocket;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using WebSocketManager;
using WebSocketManager.Common;

namespace Neon.WebApi.WebSockets
{

	[WebSocketHub("/ws/events")]
	public class EventsHub : WebSocketHandler
	{

		public EventsHub(WebSocketConnectionManager webSocketConnectionManager, IIoTService ioTService) : base(webSocketConnectionManager)
		{
			ioTService.GetEventStream<INeonIoTEntity>().Subscribe(async entity =>
			{
				await SendMessageToAllAsync(new Message()
				{
					MessageType = MessageType.Text,
					Data = entity.ToJson()
				});
			});
		}
	}
}
