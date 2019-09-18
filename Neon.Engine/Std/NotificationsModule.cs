using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Services;

namespace Neon.Engine.Std
{
	[ScriptModule]
	public class NotificationsModule
	{
		private readonly INotificationService _notificationService;

		public NotificationsModule(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		[ScriptFunction("notify_connector", "Notify connector")]
		public void Notify(string connectorName, string text, params object[] args)
		{
			_notificationService.NotifyConnector(connectorName, text, args);
		}

		[ScriptFunction("telegram", "Notify Telegram connector")]
		public void Notify(string text, params object[] args)
		{
			_notificationService.NotifyConnector("telegram", text, args);
		}
	}
}
