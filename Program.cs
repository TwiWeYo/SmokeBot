using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SmokeBot.Configuration;
using SmokeBot.Controller;
using SmokeBot.Extensions;
using System;
using Telegram.Bot;

namespace SmokeBot
{
    class Program
    {
        private static IHost _host;
        public static void Main(string[] args)
        {
            _host = new HostBuilder()
                .ConfigureHost()
                .Build();

            var config = _host.Services.GetRequiredService<IOptions<AppSettings>>();
            var bot = new TelegramBotClient(config.Value.Token);
            var manager = _host.Services.GetRequiredService<SmokeManager>().AddBot(bot);
            Console.ReadLine();
            manager.Stop();
        }
    }
}
