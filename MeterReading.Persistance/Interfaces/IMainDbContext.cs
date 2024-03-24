using MeterReading.Domain;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Persistance.Interfaces
{
    public interface IMainDbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<WaterIndication> WaterIndications { get; set; }

        public DbSet<ElectricityIndication> ElectricityIndications { get; set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
