#nullable enable

using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for customer persistence operations.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>Finds a customer by DNI.</summary>
        /// <param name="dni">The DNI to search for.</param>
        /// <returns>The customer if found; otherwise null.</returns>
        Task<Customer?> FindByDniAsync(string dni);

        /// <summary>Finds a customer by name.</summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The customer if found; otherwise null.</returns>
        Task<Customer?> FindByNameAsync(string name);

        /// <summary>Adds a new customer to the repository.</summary>
        /// <param name="customer">The customer to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(Customer customer);
    }
}
