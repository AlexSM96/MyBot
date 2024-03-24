using MeterReading.Domain.BaseEntitys;

namespace MeterReading.Domain
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public long? ChatId { get; set; }
        public long? TelegaramUserId { get; set; }

        public List<ElectricityIndication> ElectricityIndications { get; set; } = [];

        public List<WaterIndication> WaterIndications { get; set; } = [];
    }
}
