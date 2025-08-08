using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TelegramBot
{
	[Target("Telegram")]
	public class TelegramTarget : TargetWithLayout
	{
		private const int TelegramMaxMessageLength = 4096;

		[RequiredParameter]
		public List<int> ChatIds { get; set; }

		[RequiredParameter]
		public ITelegramBotClient BotClient { get; set; }

		private IEnumerable<ChatId> RealChatIds;

		protected override void InitializeTarget()
		{
			if ((!ChatIds?.Any()) ?? true)
			{
				throw new ArgumentOutOfRangeException(nameof(ChatIds), nameof(ChatIds) + " cannot be empty.");
			}

			RealChatIds = ChatIds.Select(x => new ChatId(x));

			base.InitializeTarget();
		}

		protected override void Write(AsyncLogEventInfo info)
		{
			try
			{
				Task t = this.Send(info);
				t.Wait();
			}
			catch (Exception e)
			{
				info.Continuation(e);
			}
		}

		private async Task Send(AsyncLogEventInfo info)
		{
			String message = Layout.Render(info.LogEvent);

			List<Task> tasks = new();

			foreach (var a in RealChatIds)
			{
				tasks.Add(RunSend(message, a));
			}

			await Task.WhenAll(tasks);
		}

		private async Task RunSend(string message, ChatId a)
		{
			try
			{
			//	await BotClient.SendTextMessageAsync(a, message.Substring(0, Math.Min(message.Length, TelegramMaxMessageLength)));
			}
			catch (ApiRequestException)
			{
				throw;
			}
		}
	}
}
