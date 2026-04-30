#nullable enable
using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>Input for the RentVehicle use case.</summary>
    public sealed class RentVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleInput"/> class.
        /// </summary>
        /// <param name="licensePlate">The license plate of the vehicle to rent.</param>
        /// <param name="customerName">The full name of the customer.</param>
        /// <param name="customerDni">The DNI of the customer (required).</param>
        /// <param name="startDate">The planned start date of the rental.</param>
        /// <param name="plannedEndDate">The planned end date of the rental.</param>
        public RentVehicleInput(string licensePlate, string customerName, string customerDni, DateTime startDate, DateTime plannedEndDate)
        {
            this.LicensePlate = licensePlate;
            this.CustomerName = customerName;
            this.CustomerDni = customerDni;
            this.StartDate = startDate;
            this.PlannedEndDate = plannedEndDate;
        }

        /// <summary>Gets the license plate of the vehicle to rent.</summary>
        public string LicensePlate { get; }

        /// <summary>Gets the full name of the customer.</summary>
        public string CustomerName { get; }

        /// <summary>Gets the DNI of the customer.</summary>
        public string CustomerDni { get; }

        /// <summary>Gets the planned start date of the rental.</summary>
        public DateTime StartDate { get; }

        /// <summary>Gets the planned end date of the rental.</summary>
        public DateTime PlannedEndDate { get; }
    }
}
