#nullable enable

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

        /// <summary>Gets all available vehicles.</summary>
        /// <returns>A read-only list of available vehicles.</returns>
        Task<IReadOnlyList<Vehicle>> GetAvailableAsync();

        /// <summary>Gets a vehicle by its license plate.</summary>
        /// <param name="licensePlate">The license plate.</param>
        /// <returns>The vehicle if found; otherwise null.</returns>
        Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);

        /// <summary>Gets all vehicles with their availability status.</summary>
        /// <returns>A read-only list of all vehicles.</returns>
        Task<IReadOnlyList<Vehicle>> GetAllAsync();

        /// <summary>Updates an existing vehicle in the repository.</summary>
        /// <param name="vehicle">The vehicle to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Vehicle vehicle);
    }
}
