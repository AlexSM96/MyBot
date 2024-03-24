namespace MeterReading.Core.Keyboards.Buttons
{
    public class Button
    {
        public static List<string> UpdateOrDelete { get; } = ["Обновить", "Удалить"];

        public static List<string> WaterOrElectricity { get; } = ["💧 Вода", "⚡️ Электричество"];

        public static List<string> YesOrNo { get; } = ["Да", "Нет"];
    }
}
