using MeterReading.Core;
using Telegram.Bot;

namespace MyBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var client = BotConfiguration.GetClient();
            Console.WriteLine("Запущен бот " + client.GetMeAsync().Result.FirstName);
            BotConfiguration.Initialize();
            Console.ReadLine();
        }
    }
}