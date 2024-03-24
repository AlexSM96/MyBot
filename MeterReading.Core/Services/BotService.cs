using MeterReading.Core.Keyboards;
using MeterReading.Core.Keyboards.Buttons;
using MeterReading.Core.Services.Base;
using MeterReading.Domain;
using MeterReading.Persistance.Repositories.Base;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = MeterReading.Domain.User;

namespace MeterReading.Core.Services
{
    public class BotService(IBaseRepository<User> userRepo,
        IBaseRepository<WaterIndication> waterRepo,
        IBaseRepository<ElectricityIndication> electricityRepo) : IBotService
    {
        private readonly IBaseRepository<User> _userRepo = userRepo;
        private readonly IBaseRepository<WaterIndication> _waterRepo = waterRepo;
        private readonly IBaseRepository<ElectricityIndication> _electricityRepo = electricityRepo;

        public async Task HandleUpdateAsync(ITelegramBotClient client, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(update));
                await (update switch
                {
                    { Message: { } message } => OnMessageReceived(client, message),
                    { CallbackQuery: { } callbackQuery } => OnCallbackQueryReceived(client, callbackQuery),
                    _ => OnUnknownUpdateReceived(update),
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            try
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiException => $"API exception: {apiException.ErrorCode}\n{apiException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(errorMessage);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private Task OnUnknownUpdateReceived(Update update)
        {
            Console.WriteLine($"Unknown update type {update.Type}");
            return Task.CompletedTask;
        }

        private async Task OnCallbackQueryReceived(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            if (callbackQuery is null || callbackQuery.Data is null) return;

            var chat = callbackQuery.Message?.Chat;
            string callbackData = callbackQuery.Data;
            string command = callbackData.Split(':')[0] + "_" + callbackData.Split(':')[1];
            var data = callbackData.Split(':').Skip(2).ToList();

            if (chat is not null)
            {
                await (command switch
                {
                    "Обновить_W" => Task.CompletedTask,
                    "Обновить_E" => Task.CompletedTask,
                    "Удалить_W" => DeleteWaterIndication(client, chat, data[0]),
                    "Удалить_E" => DeleteElectricityIndication(client, chat, data[0]),
                    "Да_W" => SaveWaterIndication(client, callbackQuery, data),
                    "Да_E" => SaveElectricityIndication(client, callbackQuery, data),
                    "Нет_W" => Task.CompletedTask,
                    "Нет_E" => Task.CompletedTask,
                    _ => Console.Out.WriteLineAsync("Получена неизвестная команда: " + command),
                });
            }
        }

        private async Task SaveElectricityIndication(ITelegramBotClient client, CallbackQuery callbackQuery, List<string> data)
        {
            var currentUser = _userRepo.Get().Result.FirstOrDefault(u => u.TelegaramUserId == callbackQuery.From.Id);
            await _electricityRepo.CreateAsync(new ElectricityIndication
            {
                AmountElectricity = double.Parse(data[0]),
                CreationUserId = currentUser.Id
            });

            await client.SendTextMessageAsync(callbackQuery.Message.Chat, "Данные сохранены", replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task SaveWaterIndication(ITelegramBotClient client, CallbackQuery callbackQuery, List<string> data)
        {
            var currentUser = _userRepo.Get().Result.FirstOrDefault(u => u.TelegaramUserId == callbackQuery.From.Id);
            await _waterRepo.CreateAsync(new WaterIndication
            {
                AmountColdWater = double.Parse(data[0]),
                AmountHotWater = double.Parse(data[1]),
                CreationUserId = currentUser!.Id
            });
            await client.SendTextMessageAsync(callbackQuery.Message.Chat, "Данные сохранены", replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task DeleteElectricityIndication(ITelegramBotClient client, Chat chat, string callbackData)
        {
            if (Guid.TryParse(callbackData, out Guid electricityId))
            {
                var indicationToDelete = _electricityRepo.Get().Result.FirstOrDefault(x => x.Id == electricityId);
                if (indicationToDelete == null)
                {
                    await client.SendTextMessageAsync(chat, "Показания за выбранный период не найдены!");
                    return;
                }

                int success = await _electricityRepo.DeleteAsync(electricityId);
                await (success >= 0 
                    ? client.SendTextMessageAsync(chat, $"Показания за {indicationToDelete.CreationDate.ToString("MMMM")} удалены.")
                    : throw new Exception(nameof(_electricityRepo.DeleteAsync)));
            }
        }

        private async Task DeleteWaterIndication(ITelegramBotClient client, Chat chat, string callbackData)
        {
            if (Guid.TryParse(callbackData, out Guid waterId))
            {
                var indicationToDelete = _waterRepo.Get().Result.FirstOrDefault(x => x.Id == waterId);
                if (indicationToDelete == null)
                {
                    await client.SendTextMessageAsync(chat, "Показания за выбранный период не найдены!");
                    return;
                }

                int success = await _waterRepo.DeleteAsync(waterId);
                await (success >= 0 
                    ? client.SendTextMessageAsync(chat, $"Показания за {indicationToDelete.CreationDate.ToString("MMMM")} удалены.")
                    : throw new Exception(nameof(_waterRepo.DeleteAsync)));
            }
        }

        private async Task OnMessageReceived(ITelegramBotClient client, Message message)
        {
            await (message.Text?.ToLower() switch
            {
                "/start" => AddUser(client, message),
                "/prev_w" => GetWaterIndications(client, message),
                "/prev_e" => GetElectricityIndications(client, message),
                "/add" => client.SendTextMessageAsync(message.Chat, "Что вы хотите добавить?",
                    replyMarkup: Keyboard.DrawReplyKeyboard(Button.WaterOrElectricity)),
                "💧 вода" => SendWaterMessage(client, message),
                "⚡️ электричество" => SendElecMessage(client, message),
                _ => Console.Out.WriteLineAsync("Unknown command: " + message.Text?.ToLower())
            });
        }

        private async Task SendWaterMessage(ITelegramBotClient client, Message message)
        {
            var indication = await CreateIndication(client, message, new WaterIndication(), "Введите показания ХОЛОДНОЙ воды");
            var callbackData = string.Join(":", "W", indication?.AmountColdWater, indication?.AmountHotWater);
            await client.SendTextMessageAsync(message.Chat,
                $"{indication?.ToString()}\nСохранить новые данные?",
                replyMarkup: Keyboard.DrawInlineKeyboard(Button.YesOrNo, callbackData));
        }

        private async Task SendElecMessage(ITelegramBotClient client, Message message)
        {
            var indication = await CreateIndication(client, message, new ElectricityIndication(), "Введите показания счетчика");
            var callbackData = string.Join(":", "E", indication?.AmountElectricity);
            await client.SendTextMessageAsync(message.Chat,
                $"{indication?.ToString()}\nСохранить новые данные?",
                replyMarkup: Keyboard.DrawInlineKeyboard(Button.YesOrNo, callbackData));
        }

        private async Task AddUser(ITelegramBotClient client, Message message)
        {
            int success = await _userRepo.CreateAsync(new()
            {
                FirstName = message.From?.FirstName ?? "",
                LastName = message.From?.LastName ?? "",
                UserName = message.From?.Username ?? "",
                TelegaramUserId = message.From?.Id,
                ChatId = message.Chat.Id,
            });

            await (success >= 0 ? client.SendTextMessageAsync(message.Chat, "Добро пожаловать!")
                : throw new Exception(nameof(_userRepo.CreateAsync)));
        }

        private async Task GetWaterIndications(ITelegramBotClient client, Message message)
        {
            var indications = await _waterRepo.Get();
            if (indications is null || !indications.Any())
            {
                await client.SendTextMessageAsync(message.Chat, "Здесь пока пусто.");
                return;
            }

            foreach (var item in indications)
            {
                await client.SendTextMessageAsync(message.Chat, item.ToString(),
                    replyMarkup: Keyboard.DrawInlineKeyboard(Button.UpdateOrDelete, string.Join(":", "W", item.Id)));
            }

            return;
        }

        private async Task GetElectricityIndications(ITelegramBotClient client, Message message)
        {
            var indications = await _electricityRepo.Get();
            if (indications is null || !indications.Any())
            {
                await client.SendTextMessageAsync(message.Chat, "Здесь пока пусто.");
                return;
            }

            foreach (var item in indications)
            {
                await client.SendTextMessageAsync(message.Chat, item.ToString(),
                    replyMarkup: Keyboard.DrawInlineKeyboard(Button.UpdateOrDelete, string.Join(":", "E", item.Id)));
            }

            return;
        }

        private async Task<T?> CreateIndication<T>(ITelegramBotClient client, Message message, T indication, string text, int? lastUpdate = null)
        {
            if (message.Type is not MessageType.Text) return default;
            await client.SendTextMessageAsync(message.Chat.Id, text);
            while (true)
            {
                await Task.Delay(1000);
                var currentUpdate = await client.GetUpdatesAsync(lastUpdate);
                var newUpdate = currentUpdate.LastOrDefault(x => x.Message?.Chat.Id == message.Chat.Id);
                if (newUpdate is not null && newUpdate.Message is not null && newUpdate.Message.Type == MessageType.Text)
                {
                    if (double.TryParse(newUpdate.Message?.Text?.Replace('.', ','), out double result))
                    {
                        if(indication is WaterIndication waterIndication)
                        {
                            if (waterIndication.AmountColdWater <= 0)
                            {
                                waterIndication.AmountColdWater = result;
                                await CreateIndication(client, message, indication, "Введите показания ГОРЯЧЕЙ воды", newUpdate.Id + 1);
                            }
                            else
                            {
                                waterIndication.AmountHotWater = result;
                            }
                        }

                        if(indication is ElectricityIndication electricityIndication)
                        {
                            if(electricityIndication.AmountElectricity <= 0)
                            {
                                electricityIndication.AmountElectricity = result;
                            }
                        }
                       
                        return indication;
                    }
                }
            }
        }
    }
}
