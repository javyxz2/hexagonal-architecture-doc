#nullable enable
using System;
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
        public async Task<Customer?> FindByNameAsync(string name)
        {
            return await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerName == name);
        }

        /// <inheritdoc />
        public async Task AddAsync(Customer customer)
        {
            ArgumentNullException.ThrowIfNull(customer);
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();
        }
    }
}
