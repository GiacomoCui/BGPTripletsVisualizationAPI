using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using TelegramBot;

namespace Shared.Logging
{
    public class LoggerInit
    {

        public static NLogLoggerFactory InitLogger(FileInfo outFile, bool deleteOldFile = false, bool logTelegram = true)
        {
            
            LoggingConfiguration lc = new();
            ColoredConsoleTarget ct = new();
            lc.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, ct, "*");

            if (outFile is not null)
            {
               
                if(deleteOldFile)
                    outFile.Delete();
                FileTarget ct1 = new();
                ct1.FileName = outFile.FullName;
                lc.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, ct1, "*");
            }
            if (logTelegram)
            {
                string Token = "7078489629:AAH_m9qer0U7YdIqTvDBoZbyeFECXYTG8LU";
                var BotClient = new TelegramBotClient(Token);
                List<int> AdminTelegramIds = new() { 877385119 };
                TelegramTarget telegramTarget = new()
                {
                    BotClient = BotClient,
                    ChatIds = AdminTelegramIds,
                    Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}",
                };

                lc.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, telegramTarget, "*");
            }


            LogManager.Configuration = lc;
            LogManager.ReconfigExistingLoggers();
            NLogLoggerProvider logLoggerProvider = new();
            NLogLoggerFactory logLoggerFactory = new(logLoggerProvider);
            
            return logLoggerFactory;
        }

    }
}
