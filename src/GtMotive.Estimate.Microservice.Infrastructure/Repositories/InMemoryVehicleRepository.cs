#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    public sealed class InMemoryVehicleRepository : IVehicleRepository
    {
        private readonly ConcurrentDictionary<Guid, Vehicle> _store = new();

        public Task AddAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            _store.TryAdd(vehicle.VehicleId, vehicle);
            return Task.CompletedTask;
        }

        public Task<Vehicle?> GetByIdAsync(Guid vehicleId)
        {
            _store.TryGetValue(vehicleId, out var vehicle);
            return Task.FromResult(vehicle);
        }

        public Task<IReadOnlyList<Vehicle>> GetAvailableAsync()
        {
            return Task.FromResult<IReadOnlyList<Vehicle>>(_store.Values.Where(v => v.IsAvailable).ToList());
        }

        public Task UpdateAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            _store[vehicle.VehicleId] = vehicle;
            return Task.CompletedTask;
        }
    }
}
