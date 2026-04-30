using GtMotive.Estimate.Microservice.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence.Configurations
{
    internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.CustomerId);

            builder.Property(c => c.CustomerId)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CustomerDni)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => c.CustomerDni)
                .IsUnique();
        }
    }
}
