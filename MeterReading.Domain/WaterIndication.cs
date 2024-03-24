using MeterReading.Domain.BaseEntitys;

namespace MeterReading.Domain
{
    public class WaterIndication : BaseEntity
    {
        public double AmountColdWater { get; set; }

        public double AmountHotWater { get; set; }

        public Guid CreationUserId { get; set; }

        public User? CreationUser { get; set; }

        public override string ToString() => $"Дата: {CreationDate.ToShortDateString()}\nПоказания счетчиков:\nХолодная: {AmountColdWater}\nГорячая: {AmountHotWater}";
    }
}
