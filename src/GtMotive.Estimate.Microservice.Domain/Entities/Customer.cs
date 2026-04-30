#nullable enable

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents a customer in the renting system.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        /// <param name="customerName">The full name of the customer.</param>
        /// <param name="customerDni">The DNI of the customer (optional).</param>
        public Customer(string customerName, string customerDni)
        {
            this.CustomerName = customerName;
            this.CustomerDni = customerDni;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// Required for Entity Framework Core materialization.
        /// </summary>
        protected Customer()
        {
        }

        /// <summary>Gets or sets the auto-incremented unique identifier of the customer.</summary>
        public long CustomerId { get; protected set; }

        /// <summary>Gets the full name of the customer.</summary>
        public string CustomerName { get; private set; } = string.Empty;

        /// <summary>Gets the DNI of the customer.</summary>
        public string CustomerDni { get; private set; } = string.Empty;
    }
}
