using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAllVehicles
{
    /// <summary>Output of the GetAllVehicles use case.</summary>
    public sealed class GetAllVehiclesOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllVehiclesOutput"/> class.
        /// </summary>
        /// <param name="vehicles">All vehicles with their status.</param>
        public GetAllVehiclesOutput(IReadOnlyList<VehicleStatusDto> vehicles)
        {
            this.Vehicles = vehicles;
        }

        /// <summary>Gets the list of all vehicles.</summary>
        public IReadOnlyList<VehicleStatusDto> Vehicles { get; }
    }
}
