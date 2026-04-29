#nullable enable
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// Thread-safe in-memory implementation of <see cref="IRentalRepository"/>.
    /// </summary>
    public sealed class InMemoryRentalRepository : IRentalRepository
    {
        private readonly ConcurrentDictionary<Guid, Rental> _store = new();

        /// <inheritdoc />
        public Task AddAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);
            _store.TryAdd(rental.RentalId, rental);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<Rental?> GetActiveByVehicleIdAsync(long vehicleId)
        {
            var rental = _store.Values.FirstOrDefault(r => r.VehicleId == vehicleId && r.IsActive);
            return Task.FromResult(rental);
        }

        /// <inheritdoc />
        public Task<bool> HasActiveRentalAsync(string customerId)
        {
            bool result = _store.Values.Any(r => r.CustomerId == customerId && r.IsActive);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task UpdateAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);
            _store[rental.RentalId] = rental;
            return Task.CompletedTask;
        }
    }
}
