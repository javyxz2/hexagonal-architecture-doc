using GtMotive.Estimate.Microservice.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence.Configurations
{
    internal sealed class RentalConfiguration : IEntityTypeConfiguration<Rental>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Rental> builder)
        {
            builder.HasKey(r => r.RentalId);

            builder.Property(r => r.VehicleId)
                .IsRequired();

            builder.Property(r => r.CustomerId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.StartDate)
                .IsRequired();

            builder.Property(r => r.PlannedEndDate)
                .IsRequired();

            builder.Property(r => r.ReturnedDate);

            // IsActive is a computed property — not persisted.
            builder.Ignore(r => r.IsActive);

            builder.HasIndex(r => new { r.VehicleId, r.ReturnedDate });
        }
    }
}
