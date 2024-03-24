namespace MeterReading.Domain.BaseEntitys
{
    public class BaseEntity
    {
        public Guid Id { get; } = Guid.NewGuid();

        public DateTime CreationDate { get; } = DateTime.Now;
    }
}
