using MeterReading.Domain.BaseEntitys;

namespace MeterReading.Domain
{
    public class ElectricityIndication : BaseEntity
    {
        public double AmountElectricity { get; set; }

        public Guid CreationUserId { get; set; }

        public User? CreationUser { get; set; }

        public override string ToString() => $"Дата: {CreationDate.ToShortDateString()}\nПоказания счетчика: {AmountElectricity}";
    }
}
