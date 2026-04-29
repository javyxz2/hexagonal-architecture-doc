using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>Output for the ReturnVehicle use case.</summary>
    public sealed class ReturnVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleOutput"/> class.
        /// </summary>
        /// <param name="rentalId">Rental identifier.</param>
        /// <param name="returnedDate">Actual return date.</param>
        public ReturnVehicleOutput(Guid rentalId, DateTime returnedDate)
        {
            this.RentalId = rentalId;
            this.ReturnedDate = returnedDate;
        }

        /// <summary>Gets the rental identifier.</summary>
        public Guid RentalId { get; }

        /// <summary>Gets the actual return date.</summary>
        public DateTime ReturnedDate { get; }
    }
}
