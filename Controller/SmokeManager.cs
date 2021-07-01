using Microsoft.Extensions.Logging;
using SmokeBot.Model;
using SmokeBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SmokeBot.Controller
{
    public class SmokeManager : BaseManager
    {
        private readonly ILogger<SmokeManager> logger;
        private readonly ICollectionService<Room> roomService;
        private readonly RoomManager roomManager;
        public Dictionary<long, Room> SmokeChats { get; }
        private CancellationTokenSource cts;
        public SmokeManager(ILogger<SmokeManager> logger, ICollectionService<Room> collectionService
            , RoomManager manager)
        {
            this.logger = logger;
            roomService = collectionService;
            roomManager = manager;
            SmokeChats = roomService.Rooms;
            cts = new CancellationTokenSource();
        }

        public override SmokeManager AddBot(ITelegramBotClient bot)
        {
            base.AddBot(bot);
            roomManager.AddBot(bot);
            Bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);
            logger.LogInformation("Я родился");
            return this;
        }

        public void Stop()
        {
            roomService.Save();
            cts.Cancel();
        }

        private Task HandleErrorAsync(ITelegramBotClient client, Exception ex, CancellationToken cancellationToken)
        {
            var ErrorMessage = ex switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => ex.ToString()
            };

            logger.LogError(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message),
                _ => Task.Run(() => logger.LogInformation($"Я ещё не умею обрабатывать {update.Type}"))
            };
            try
            {
                await handler;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(client, ex, cancellationToken);
            }
        }

        private Room HandleRooms(long id)
        {
            Room room;
            if (!roomService.Rooms.ContainsKey(id))
            {
                roomService.Rooms.Add(id, room = new Room(id));
                logger.LogDebug($"Added new Smoking room with id: {id}");
                return room;
            }
            return roomService.Rooms[id];
        }

        private async Task BotOnMessageReceived(Message message)
        {
            var room = HandleRooms(message.Chat.Id);
            
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            if (!message.Text.StartsWith('/'))
                return;
            var action = (message.Text.Split(' ').First()) switch
            {
                "/interval" => roomManager.SetInterval(message, room),
                "/starthour" => roomManager.SetStartHour(message, room),
                "/endhour" => roomManager.SetEndHour(message, room),
                _ => Usage(message)
            };
            var sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
        }

        private async Task<Message> Usage(Message message)
        {
            const string usage = "Команды:\n" +
                "/interval <minutes>:  Задает интервал перекуров\n" +
                "/starthour <hour>:  Задает начальное время перекуров\n" +
                "/endhour <hour>:  Задает конечное время перекуров\n";
            return await SendMessage(message.Chat.Id, usage);
        }
    }
}
