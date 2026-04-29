#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for vehicle persistence operations.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>Adds a new vehicle to the repository.</summary>
        /// <param name="vehicle">The vehicle to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(Vehicle vehicle);

        /// <summary>Gets a vehicle by its identifier.</summary>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <returns>The vehicle if found; otherwise null.</returns>
        Task<Vehicle?> GetByIdAsync(Guid vehicleId);

        /// <summary>Gets all available vehicles.</summary>
        /// <returns>A read-only list of available vehicles.</returns>
        Task<IReadOnlyList<Vehicle>> GetAvailableAsync();

        /// <summary>Updates an existing vehicle in the repository.</summary>
        /// <param name="vehicle">The vehicle to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Vehicle vehicle);
    }
}
