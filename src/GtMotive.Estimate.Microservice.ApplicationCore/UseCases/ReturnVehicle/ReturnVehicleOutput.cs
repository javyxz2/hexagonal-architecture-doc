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
        /// <param name="endDate">End date of the rental.</param>
        public ReturnVehicleOutput(Guid rentalId, DateTime endDate)
        {
            RentalId = rentalId;
            EndDate = endDate;
        }

        /// <summary>Gets the rental identifier.</summary>
        public Guid RentalId { get; }

        /// <summary>Gets the end date of the rental.</summary>
        public DateTime EndDate { get; }
    }
}
