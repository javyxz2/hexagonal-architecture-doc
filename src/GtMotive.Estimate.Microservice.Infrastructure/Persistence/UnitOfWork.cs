#nullable enable
using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core implementation of <see cref="IUnitOfWork"/>.
    /// Wraps <see cref="RentingDbContext"/> to provide transactional control.
    /// </summary>
    public sealed class UnitOfWork(RentingDbContext context) : IUnitOfWork, IDisposable
    {
        private IDbContextTransaction? _transaction;

        /// <inheritdoc />
        public async Task BeginTransactionAsync()
        {
            _transaction = await context.Database.BeginTransactionAsync();
        }

        /// <inheritdoc />
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        /// <inheritdoc />
        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
