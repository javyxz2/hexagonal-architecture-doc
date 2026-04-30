#nullable enable
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="ICustomerRepository"/>.
    /// </summary>
    public sealed class CustomerRepository(RentingDbContext context) : ICustomerRepository
    {
        /// <inheritdoc />
        public async Task<Customer?> FindByDniAsync(string dni)
        {
            return await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerDni == dni);
        }

        /// <inheritdoc />
        public async Task<Customer> FindOrCreateAsync(string customerName, string customerDni)
        {
            var existing = await FindByDniAsync(customerDni);
            if (existing != null)
            {
                return existing;
            }

            var customer = new Customer(customerName, customerDni);

            try
            {
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
                return customer;
            }
            catch (DbUpdateException)
            {
                // Concurrent request inserted the same DNI first — clear the failed
                // tracked entity and fetch the one already in the database.
                context.ChangeTracker.Clear();
                return await context.Customers
                    .FirstAsync(c => c.CustomerDni == customerDni);
            }
        }
    }
}
