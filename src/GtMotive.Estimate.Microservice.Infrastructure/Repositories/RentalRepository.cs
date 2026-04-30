#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IRentalRepository"/>.
    /// </summary>
    public sealed class RentalRepository(RentingDbContext context) : IRentalRepository
    {
        /// <inheritdoc />
        public async Task AddAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);
            await context.Rentals.AddAsync(rental);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Rental?> GetActiveByVehicleIdAsync(long vehicleId)
        {
            return await context.Rentals
                .Where(r => r.VehicleId == vehicleId && r.ReturnedDate == null)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasActiveRentalAsync(long customerId)
        {
            return await context.Rentals
                .AnyAsync(r => r.CustomerId == customerId && r.ReturnedDate == null);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);
            context.Rentals.Update(rental);
            await context.SaveChangesAsync();
        }
    }
}
