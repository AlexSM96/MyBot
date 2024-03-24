using Telegram.Bot;
using Telegram.Bot.Polling;

namespace MeterReading.Core
{
    public static class BotConfiguration
    {
        private static DipendencyInjection dipendencyInjection = new DipendencyInjection();
        
        public static ITelegramBotClient GetClient(string token = "")
        {
            token = string.IsNullOrWhiteSpace(token) ?
                dipendencyInjection.Configuration.GetSection("Bot").GetSection("Token").Value 
                ?? throw new Exception("Не найден токен телеграм бота") : token;
            return new TelegramBotClient(token);
        }
        
        public static void Initialize()
        {
            var client = GetClient();
            client.StartReceiving(dipendencyInjection.BotService.HandleUpdateAsync, 
                dipendencyInjection.BotService.HandleErrorAsync, 
                new ReceiverOptions
                {
                    AllowedUpdates = { }
                });
        }
    }
}
