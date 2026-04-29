using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles
{
    /// <summary>Output for the GetAvailableVehicles use case.</summary>
    public sealed class GetAvailableVehiclesOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableVehiclesOutput"/> class.
        /// </summary>
        /// <param name="vehicles">List of available vehicles.</param>
        public GetAvailableVehiclesOutput(IReadOnlyList<VehicleDto> vehicles)
        {
            Vehicles = vehicles;
        }

        /// <summary>Gets the list of available vehicles.</summary>
        public IReadOnlyList<VehicleDto> Vehicles { get; }
    }
}
