namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>Input for the ReturnVehicle use case.</summary>
    public sealed class ReturnVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleInput"/> class.
        /// </summary>
        /// <param name="licensePlate">The license plate of the vehicle to return.</param>
        public ReturnVehicleInput(string licensePlate)
        {
            this.LicensePlate = licensePlate;
        }

        /// <summary>Gets the license plate of the vehicle.</summary>
        public string LicensePlate { get; }
    }
}
