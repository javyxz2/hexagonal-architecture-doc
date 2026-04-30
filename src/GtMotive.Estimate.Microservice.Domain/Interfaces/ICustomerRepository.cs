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

        /// <summary>
        /// Returns the existing customer with the given DNI, or creates a new one atomically.
        /// Safe against concurrent inserts: if two requests race, one will find the record
        /// inserted by the other instead of failing.
        /// </summary>
        /// <param name="customerName">Full name of the customer.</param>
        /// <param name="customerDni">Unique DNI of the customer.</param>
        /// <returns>The existing or newly created customer.</returns>
        Task<Customer> FindOrCreateAsync(string customerName, string customerDni);
    }
}
