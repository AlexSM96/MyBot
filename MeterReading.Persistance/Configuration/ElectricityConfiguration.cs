using MeterReading.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReading.Persistance.Configuration
{
    internal class ElectricityConfiguration : IEntityTypeConfiguration<ElectricityIndication>
    {
        public void Configure(EntityTypeBuilder<ElectricityIndication> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id);

            builder
                .HasOne(ei => ei.CreationUser)
                .WithMany(u => u.ElectricityIndications)
                .HasForeignKey(ei => ei.CreationUserId);
        }
    }
}
