using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Notifiers;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.NoSql;
using Neon.Api.Interfaces.Notifiers;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using Neon.Engine.Notifiers.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Neon.Engine.Notifiers
{
	[Notifier("telegram", ConfigType = typeof(TelegramNotifierConfig))]
	public class TelegramNotifier : INotifier
	{
		private TelegramNotifierConfig _telegramConfig;
		private ITelegramBotClient _telegramBotClient;
		private readonly HttpClient _httpClient;
		private readonly INoSqlService _noSqlService;
		private INoSqlConnector _persistenceConnector;
		private IScriptEngineService _scriptEngineService;
		private readonly NeonConfig _neonConfig;

		private readonly ILogger _logger;
		public TelegramNotifier(ILogger<TelegramNotifier> logger, INoSqlService noSqlService, NeonConfig neonConfig, IScriptEngineService scriptEngineService)
		{
			_logger = logger;
			_scriptEngineService = scriptEngineService;
			_neonConfig = neonConfig;
			_noSqlService = noSqlService;
			_httpClient = new HttpClient();
		}

		public async Task<bool> Notify(string text, params object[] args)
		{
			var toNotify = _persistenceConnector.Query<TelegramNotifierEntity>("notifiers").Where(e => e.IsEnabled).ToList();

			foreach (var entity in toNotify)
			{
				await _telegramBotClient.SendTextMessageAsync(entity.ChatId, text, ParseMode.Markdown);
			}

			return true;
		}

		public Task<bool> Init(object config)
		{
			_telegramConfig = (TelegramNotifierConfig)config;

			return Task.FromResult(true);
		}

		public async Task<bool> Start()
		{
			if (_telegramConfig.ApiKey == "change_me")
			{
				_logger.LogError($"Telegram notifier need api on config!");

				return false;
			}
			else
			{
				_persistenceConnector = _noSqlService.GetNoSqlConnector(_telegramConfig.PersistenceConnector);
				await _persistenceConnector.Configure(Path.Combine(_neonConfig.NotifierConfig.DirectoryConfig.DirectoryName,
					"telegram.json"));
				_telegramBotClient = new TelegramBotClient(_telegramConfig.ApiKey);
				_logger.LogInformation($"Connecting to telegram");
				var user = await _telegramBotClient.GetMeAsync();

				_logger.LogInformation($"Telegram connected, Write me @{user.Username}");

				_telegramBotClient.OnMessage += async (sender, args) =>
				{
					if (args.Message.Text != null)
					{
						_logger.LogDebug($"Message from @{args.Message.From.Username}: {args.Message.Text}");

						if (args.Message.Text.ToLower() == "/version")
							await SendVersion(args.Message.Chat.Id);

						if (args.Message.Text.ToLower() == "/help")
							await SendHelp(args.Message.Chat.Id);

						if (args.Message.Text.ToLower() == "/myip")
							await SendMyIp(args.Message.Chat.Id);

						if (args.Message.Text.ToLower().StartsWith("/exec"))
							await ExecuteCode(args.Message.Chat.Id, args.Message.Text);

						if (args.Message.Text.ToLower() == "/enable_notify")
							await EnableNotification(args.Message.Chat.Id);
					}
				};
				_telegramBotClient.StartReceiving();
			}

			return true;
		}

		private async Task SendVersion(ChatId chat)
		{
			await _telegramBotClient.SendTextMessageAsync(chat, $"Neon v{AssemblyUtils.GetVersion()}");
		}

		private async Task ExecuteCode(ChatId chat, string message)
		{
			message = message.Replace("/exec", "");

			try
			{
				var obj = _scriptEngineService.ExecuteCode(message);

				await _telegramBotClient.SendTextMessageAsync(chat, $"{obj}",
					ParseMode.Markdown);

			}
			catch (Exception ex)
			{
				await _telegramBotClient.SendTextMessageAsync(chat, $"Error during execute code: *{ex.Message}*",
					ParseMode.Markdown);
			}
		}

		private async Task SendMyIp(ChatId chat)
		{
			try
			{
				var ipAddress = await _httpClient.GetStringAsync("https://api.ipify.org");
				await _telegramBotClient.SendTextMessageAsync(chat, $"Ip Address is: *{ipAddress}*",
					ParseMode.Markdown);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during get IP address: {ex.Message}");
				await _telegramBotClient.SendTextMessageAsync(chat, $"Error: *{ex.Message}*",
					ParseMode.Markdown);
			}
		}

		private async Task EnableNotification(ChatId chatId)
		{
			var entity = _persistenceConnector.Query<TelegramNotifierEntity>("notifiers")
				.FirstOrDefault(c => c.ChatId.Identifier == chatId.Identifier);

			if (entity == null)
			{
				entity = new TelegramNotifierEntity()
				{
					IsEnabled = true,
					ChatId = chatId
				};

				_persistenceConnector.Insert("notifiers", entity);

			}
			else
			{
				entity.IsEnabled = true;
				_persistenceConnector.Update("notifiers", entity);
			}

			await _telegramBotClient.SendTextMessageAsync(chatId, "Notification Enabled");
		}

		private async Task SendHelp(ChatId chat)
		{
			var commands = new StringBuilder();

			commands.Append($"*/version*: Show Neon version\n");
			commands.Append($"*/help*: Show this message\n");
			commands.Append($"*/myip*: Show home Ip Address\n");
			commands.Append($"*/enable_notify*: Enable notification for current user\n");


			await _telegramBotClient.SendTextMessageAsync(chat, commands.ToString(), ParseMode.Markdown);
		}

		public object GetDefaultConfig()
		{
			return new TelegramNotifierConfig();
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
			_telegramBotClient?.StopReceiving();
		}
	}
}
