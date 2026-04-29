using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>Input for the RentVehicle use case.</summary>
    public sealed class RentVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleInput"/> class.
        /// </summary>
        /// <param name="vehicleId">The vehicle to rent.</param>
        /// <param name="customerId">The customer renting the vehicle.</param>
        /// <param name="startDate">The planned start date of the rental.</param>
        /// <param name="plannedEndDate">The planned end date of the rental.</param>
        public RentVehicleInput(long vehicleId, string customerId, DateTime startDate, DateTime plannedEndDate)
        {
            this.VehicleId = vehicleId;
            this.CustomerId = customerId;
            this.StartDate = startDate;
            this.PlannedEndDate = plannedEndDate;
        }

        /// <summary>Gets the vehicle identifier.</summary>
        public long VehicleId { get; }

        /// <summary>Gets the customer identifier.</summary>
        public string CustomerId { get; }

        /// <summary>Gets the planned start date of the rental.</summary>
        public DateTime StartDate { get; }

        /// <summary>Gets the planned end date of the rental.</summary>
        public DateTime PlannedEndDate { get; }
    }
}
