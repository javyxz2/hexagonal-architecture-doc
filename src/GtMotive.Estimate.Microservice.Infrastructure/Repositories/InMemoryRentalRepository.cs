#nullable enable
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    public sealed class InMemoryRentalRepository : IRentalRepository
    {
        private readonly ConcurrentDictionary<Guid, Rental> _store = new();

        public Task AddAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);
            _store.TryAdd(rental.RentalId, rental);
            return Task.CompletedTask;
        }

        public Task<Rental?> GetActiveByVehicleIdAsync(Guid vehicleId)
        {
            var rental = _store.Values.FirstOrDefault(r => r.VehicleId == vehicleId && r.IsActive);
            return Task.FromResult(rental);
        }

        public Task<bool> HasActiveRentalAsync(string customerId)
        {
            bool result = _store.Values.Any(r => r.CustomerId == customerId && r.IsActive);
            return Task.FromResult(result);
        }

        public Task UpdateAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);
            _store[rental.RentalId] = rental;
            return Task.CompletedTask;
        }
    }
}
