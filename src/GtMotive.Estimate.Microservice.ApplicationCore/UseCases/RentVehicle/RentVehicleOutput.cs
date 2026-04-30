using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>Output for the RentVehicle use case.</summary>
    public sealed class RentVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleOutput"/> class.
        /// </summary>
        /// <param name="rentalId">Rental identifier.</param>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="customerId">Auto-incremented customer identifier.</param>
        /// <param name="startDate">Planned start date of the rental.</param>
        /// <param name="plannedEndDate">Planned end date of the rental.</param>
        public RentVehicleOutput(Guid rentalId, long vehicleId, long customerId, DateTime startDate, DateTime plannedEndDate)
        {
            this.RentalId = rentalId;
            this.VehicleId = vehicleId;
            this.CustomerId = customerId;
            this.StartDate = startDate;
            this.PlannedEndDate = plannedEndDate;
        }

        /// <summary>Gets the rental identifier.</summary>
        public Guid RentalId { get; }

        /// <summary>Gets the vehicle identifier.</summary>
        public long VehicleId { get; }

        /// <summary>Gets the auto-incremented customer identifier.</summary>
        public long CustomerId { get; }

        /// <summary>Gets the planned start date of the rental.</summary>
        public DateTime StartDate { get; }

        /// <summary>Gets the planned end date of the rental.</summary>
        public DateTime PlannedEndDate { get; }
    }
}
