using MeterReading.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReading.Persistance.Configuration
{
    internal class WaterConfiguration : IEntityTypeConfiguration<WaterIndication>
    {
        public void Configure(EntityTypeBuilder<WaterIndication> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);

            builder
                .HasOne(wi => wi.CreationUser)
                .WithMany(u => u.WaterIndications)
                .HasForeignKey(wi => wi.CreationUserId);
        }
    }
}
