using MeterReading.Domain;
using MeterReading.Persistance.Configuration;
using MeterReading.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Persistance.DatabaseContext
{
    public class MainDbContext : DbContext, IMainDbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<WaterIndication> WaterIndications { get; set; }

        public DbSet<ElectricityIndication> ElectricityIndications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new UserConfiguration())
                .ApplyConfiguration(new WaterConfiguration())
                .ApplyConfiguration(new ElectricityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
