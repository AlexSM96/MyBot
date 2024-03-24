using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReading.Core.Keyboards
{
    internal static class Keyboard
    {
        public static InlineKeyboardMarkup DrawInlineKeyboard(List<string> names, string id)
        {
            var buttons = new List<InlineKeyboardButton>();
            foreach (var name in names)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(name, $"{name}:{id}"));
            }

            return new InlineKeyboardMarkup(buttons);
        }

        public static ReplyKeyboardMarkup DrawReplyKeyboard(List<string> buttonsName)
        {
            List<KeyboardButton> buttons = [];

            foreach (var name in buttonsName)
            {
                buttons.Add(new KeyboardButton(name));
            }

            var keyboard = new ReplyKeyboardMarkup(buttons);
            keyboard.ResizeKeyboard = true;
            return keyboard;
        }
    }
}
