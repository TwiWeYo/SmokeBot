using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SmokeBot.Controller
{
    public class BaseManager
    {
        public ITelegramBotClient Bot { get; protected set; }

        public virtual BaseManager AddBot(ITelegramBotClient bot)
        {
            Bot = bot;
            return this;
        }

        public async Task<Message> SendMessage(long id, string message)
        {
            return await Bot.SendTextMessageAsync(
                chatId: id,
                text: message);
        }
    }
}
