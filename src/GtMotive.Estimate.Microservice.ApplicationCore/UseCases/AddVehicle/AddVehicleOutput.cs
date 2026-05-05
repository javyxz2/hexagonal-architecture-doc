namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle
{
    /// <summary>Output for the AddVehicle use case.</summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AddVehicleOutput"/> class.
    /// </remarks>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="brand">Vehicle brand.</param>
    /// <param name="model">Vehicle model.</param>
    /// <param name="licensePlate">License plate.</param>
    /// <param name="manufactureYear">Year of manufacture.</param>
    public sealed class AddVehicleOutput(long vehicleId, string brand, string model, string licensePlate, int manufactureYear) : IUseCaseOutput
    {
        /// <summary>Gets the vehicle identifier.</summary>
        public long VehicleId { get; } = vehicleId;

        /// <summary>Gets the vehicle brand.</summary>
        public string Brand { get; } = brand;

        /// <summary>Gets the vehicle model.</summary>
        public string Model { get; } = model;

        /// <summary>Gets the license plate.</summary>
        public string LicensePlate { get; } = licensePlate;

        /// <summary>Gets the year of manufacture.</summary>
        public int ManufactureYear { get; } = manufactureYear;
    }
}
