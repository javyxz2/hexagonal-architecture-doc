using GtMotive.Estimate.Microservice.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence.Configurations
{
    internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.VehicleId);

            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.LicensePlate)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(v => v.LicensePlate)
                .IsUnique();

            builder.Property(v => v.ManufactureYear)
                .IsRequired();

            builder.Property(v => v.IsAvailable)
                .IsRequired();
        }
    }
}
