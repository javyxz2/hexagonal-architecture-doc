namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle
{
    /// <summary>Input for the AddVehicle use case.</summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AddVehicleInput"/> class.
    /// </remarks>
    /// <param name="brand">Vehicle brand.</param>
    /// <param name="model">Vehicle model.</param>
    /// <param name="licensePlate">License plate.</param>
    /// <param name="manufactureYear">Year of manufacture.</param>
    public sealed class AddVehicleInput(string brand, string model, string licensePlate, int manufactureYear) : IUseCaseInput
    {
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
