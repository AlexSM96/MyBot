using Telegram.Bot;
using Telegram.Bot.Types;

namespace MeterReading.Core.Services.Base
{
    public interface IBotService
    {
        public Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken);

        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken);
    } 
}
