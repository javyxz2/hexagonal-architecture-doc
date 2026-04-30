#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IVehicleRepository"/>.
    /// </summary>
    public sealed class VehicleRepository(RentingDbContext context) : IVehicleRepository
    {
        /// <inheritdoc />
        public async Task AddAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            await context.Vehicles.AddAsync(vehicle);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Vehicle?> GetByIdAsync(long vehicleId)
        {
            return await context.Vehicles.FindAsync(vehicleId);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Vehicle>> GetAvailableAsync()
        {
            return await context.Vehicles
                .Where(v => v.IsAvailable)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
        {
            return await context.Vehicles
                .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Vehicle>> GetAllAsync()
        {
            return await context.Vehicles
                .OrderBy(v => v.VehicleId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            context.Vehicles.Update(vehicle);
            await context.SaveChangesAsync();
        }
    }
}
