#nullable enable

using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for rental persistence operations.
    /// </summary>
    public interface IRentalRepository
    {
        /// <summary>Adds a new rental to the repository.</summary>
        /// <param name="rental">The rental to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(Rental rental);

        /// <summary>Gets the active rental for a specific vehicle.</summary>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <returns>The active rental if found; otherwise null.</returns>
        Task<Rental?> GetActiveByVehicleIdAsync(long vehicleId);

        /// <summary>Determines whether a customer has an active rental.</summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>True if the customer has an active rental; otherwise false.</returns>
        Task<bool> HasActiveRentalAsync(long customerId);

        /// <summary>Updates an existing rental in the repository.</summary>
        /// <param name="rental">The rental to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Rental rental);
    }
}
