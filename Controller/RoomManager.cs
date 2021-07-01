using Microsoft.Extensions.Logging;
using SmokeBot.Model;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SmokeBot.Controller
{
    public class RoomManager : BaseManager
    {
        private readonly ILogger<RoomManager> logger;
        public RoomManager(ILogger<RoomManager> logger)
        {
            this.logger = logger;
        }

        public override RoomManager AddBot(ITelegramBotClient bot)
        {
            base.AddBot(bot);
            return this;
        }

        public async Task<Message> SetInterval(Message message, Room room)
        {
            var max = Math.Abs(room.EndHour - room.StartHour);
            if (room.StartHour == room.EndHour)
                max = 24;
            var outMessage = ParseTime(message, 0, max * 60, out int time);
            if (time >= 0)
            {
                room.Interval = time;
                logger.LogInformation($"В чате {room.Id} установлен интервал: {time}");
            }
            return await SendMessage(room.Id, outMessage);
        }

        public async Task<Message> SetStartHour(Message message, Room room)
        {
            var outMessage = ParseTime(message, 0, 23, out int time);
            if (time >= 0)
            {
                room.StartHour = time;
                logger.LogInformation($"В чате {room.Id} установлено начальное время: {time}");
            }
            return await SendMessage(room.Id, outMessage);
        }

        public async Task<Message> SetEndHour(Message message, Room room)
        {
            var outMessage = ParseTime(message, 0, 23, out int time);
            if (time >= 0)
            {
                room.EndHour = time;
                logger.LogInformation($"В чате {room.Id} установлено конечное время: {time}");
            }
            return await SendMessage(room.Id, outMessage);
        }

        private string ParseTime(Message message, int min, int max, out int time)
        {
            time = -1;
            string outMessage;
            
            var words = message.Text.Split(' ');
            if (words.Length < 2)
                return "Параметр не указан";

            if (int.TryParse(words[1], out int interval))
            {
                if (interval < min)
                    outMessage = "Значение не может быть меньше нуля";
                else if (interval > max)
                    outMessage = "Куда ты столько ставишь?";
                else
                {
                    time = interval;
                    outMessage = $"Установлено значение: {interval}";
                }
            }
            else
                outMessage = "Пишите число цифрами, а не буквами";
            return outMessage;
        }
    }
}
