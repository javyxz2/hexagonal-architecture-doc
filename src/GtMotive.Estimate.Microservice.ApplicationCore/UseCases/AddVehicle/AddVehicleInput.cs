namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle
{
    /// <summary>Input for the AddVehicle use case.</summary>
    public sealed class AddVehicleInput : IUseCaseInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddVehicleInput"/> class.
        /// </summary>
        /// <param name="brand">Vehicle brand.</param>
        /// <param name="model">Vehicle model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureYear">Year of manufacture.</param>
        public AddVehicleInput(string brand, string model, string licensePlate, int manufactureYear)
        {
            Brand = brand;
            Model = model;
            LicensePlate = licensePlate;
            ManufactureYear = manufactureYear;
        }

        /// <summary>Gets the vehicle brand.</summary>
        public string Brand { get; }

        /// <summary>Gets the vehicle model.</summary>
        public string Model { get; }

        /// <summary>Gets the license plate.</summary>
        public string LicensePlate { get; }

        /// <summary>Gets the year of manufacture.</summary>
        public int ManufactureYear { get; }
    }
}
