using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Unit Of Work. Should only be used by Use Cases.
    /// Provides transactional control over multiple repository operations.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>Applies all pending database changes within the current transaction.</summary>
        /// <returns>Number of affected rows.</returns>
        Task<int> SaveAsync();

        /// <summary>Begins a new database transaction.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>Commits the current transaction, persisting all changes atomically.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommitAsync();

        /// <summary>Rolls back the current transaction, discarding all pending changes.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RollbackAsync();
    }
}
