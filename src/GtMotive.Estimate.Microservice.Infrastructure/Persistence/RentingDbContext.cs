using System;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core database context for the renting microservice.
    /// </summary>
    public sealed class RentingDbContext(DbContextOptions<RentingDbContext> options) : DbContext(options)
    {
        /// <summary>Gets the vehicles table.</summary>
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        /// <summary>Gets the customers table.</summary>
        public DbSet<Customer> Customers => Set<Customer>();

        /// <summary>Gets the rentals table.</summary>
        public DbSet<Rental> Rentals => Set<Rental>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new RentalConfiguration());
        }
    }
}
