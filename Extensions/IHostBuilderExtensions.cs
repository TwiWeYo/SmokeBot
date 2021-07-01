using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using SmokeBot.Configuration;
using SmokeBot.Services;
using SmokeBot.Model;
using SmokeBot.Controller;

namespace SmokeBot.Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder ConfigureHost(this IHostBuilder builder)
        {
            return builder
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json");
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging
                        .AddConsole()
                        .AddDebug();
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .Configure<AppSettings>(context.Configuration)

                        .AddSingleton<SmokeManager>()
                        .AddSingleton<RoomManager>()
                        .AddSingleton<ICollectionService<Room>, RoomCollectionService>()
                        .AddSingleton<ISerializer<Room>, JsonSerializer>();
                });
        }
    }
}
