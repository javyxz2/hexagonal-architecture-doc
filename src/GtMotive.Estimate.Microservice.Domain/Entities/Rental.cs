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
        /// <param name="customerId">The auto-incremented customer identifier.</param>
        /// <param name="startDate">The planned start date of the rental.</param>
        /// <param name="plannedEndDate">The planned end date of the rental.</param>
        public Rental(long vehicleId, long customerId, DateTime startDate, DateTime plannedEndDate)
        {
            RentalId = Guid.NewGuid();
            VehicleId = vehicleId;
            CustomerId = customerId;
            StartDate = startDate;
            PlannedEndDate = plannedEndDate;
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
        public long VehicleId { get; private set; }

        /// <summary>Gets the auto-incremented identifier of the customer.</summary>
        public long CustomerId { get; private set; }

        /// <summary>Gets the planned start date of the rental.</summary>
        public DateTime StartDate { get; private set; }

        /// <summary>Gets the planned end date of the rental.</summary>
        public DateTime PlannedEndDate { get; private set; }

        /// <summary>Gets the actual return date, or null if still active.</summary>
        public DateTime? ReturnedDate { get; private set; }

        /// <summary>Gets a value indicating whether the rental is still active.</summary>
        public bool IsActive => ReturnedDate == null;

        /// <summary>Completes the rental by recording the actual return date.</summary>
        public void Complete() => ReturnedDate = DateTime.UtcNow;
    }
}
