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
        public RentVehicleInput(Guid vehicleId, string customerId)
        {
            VehicleId = vehicleId;
            CustomerId = customerId;
        }

        /// <summary>Gets the vehicle identifier.</summary>
        public Guid VehicleId { get; }

        /// <summary>Gets the customer identifier.</summary>
        public string CustomerId { get; }
    }
}
