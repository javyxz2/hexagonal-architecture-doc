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
        /// <param name="customerId">Customer identifier.</param>
        /// <param name="startDate">Start date of the rental.</param>
        public RentVehicleOutput(Guid rentalId, Guid vehicleId, string customerId, DateTime startDate)
        {
            RentalId = rentalId;
            VehicleId = vehicleId;
            CustomerId = customerId;
            StartDate = startDate;
        }

        /// <summary>Gets the rental identifier.</summary>
        public Guid RentalId { get; }

        /// <summary>Gets the vehicle identifier.</summary>
        public Guid VehicleId { get; }

        /// <summary>Gets the customer identifier.</summary>
        public string CustomerId { get; }

        /// <summary>Gets the start date of the rental.</summary>
        public DateTime StartDate { get; }
    }
}
