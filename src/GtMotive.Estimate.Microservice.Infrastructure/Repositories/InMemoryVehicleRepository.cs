#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// Thread-safe in-memory implementation of <see cref="IVehicleRepository"/>.
    /// </summary>
    public sealed class InMemoryVehicleRepository : IVehicleRepository
    {
        private static long _nextId;
        private readonly ConcurrentDictionary<long, Vehicle> _store = new();

        /// <inheritdoc />
        public Task AddAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            vehicle.AssignId(Interlocked.Increment(ref _nextId));
            _store.TryAdd(vehicle.VehicleId, vehicle);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<Vehicle?> GetByIdAsync(long vehicleId)
        {
            _store.TryGetValue(vehicleId, out var vehicle);
            return Task.FromResult(vehicle);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Vehicle>> GetAvailableAsync()
        {
            return Task.FromResult<IReadOnlyList<Vehicle>>(_store.Values.Where(v => v.IsAvailable).ToList());
        }

        /// <inheritdoc />
        public Task UpdateAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            _store[vehicle.VehicleId] = vehicle;
            return Task.CompletedTask;
        }
    }
}
