using System;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents a vehicle rental record.
    /// </summary>
    public class Rental
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// </summary>
        /// <param name="vehicleId">The vehicle being rented.</param>
        /// <param name="customerId">The customer renting the vehicle.</param>
        public Rental(Guid vehicleId, string customerId)
        {
            RentalId = Guid.NewGuid();
            VehicleId = vehicleId;
            CustomerId = customerId;
            StartDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// Required for Entity Framework Core materialization.
        /// </summary>
        protected Rental()
        {
        }

        /// <summary>Gets the unique identifier of the rental.</summary>
        public Guid RentalId { get; private set; }

        /// <summary>Gets the identifier of the rented vehicle.</summary>
        public Guid VehicleId { get; private set; }

        /// <summary>Gets the identifier of the customer.</summary>
        public string CustomerId { get; private set; }

        /// <summary>Gets the start date of the rental.</summary>
        public DateTime StartDate { get; private set; }

        /// <summary>Gets the end date of the rental, or null if still active.</summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>Gets a value indicating whether the rental is still active.</summary>
        public bool IsActive => EndDate == null;

        /// <summary>Completes the rental by setting the end date.</summary>
        public void Complete() => EndDate = DateTime.UtcNow;
    }
}
