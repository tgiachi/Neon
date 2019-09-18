using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Utils;
using Telegram.Bot.Types;

namespace Neon.Engine.Notifiers.Entities
{
	public class TelegramNotifierEntity : INeonEntity
	{
		public string Id { get; set; }

		public ChatId ChatId { get; set; }

		public bool IsEnabled { get; set; }

		public TelegramNotifierEntity()
		{
			Id = EntitiesUtils.GenerateId();
			IsEnabled = true;
		}
	}
}
