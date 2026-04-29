namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle
{
    /// <summary>Output for the AddVehicle use case.</summary>
    public sealed class AddVehicleOutput : IUseCaseOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddVehicleOutput"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="brand">Vehicle brand.</param>
        /// <param name="model">Vehicle model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureYear">Year of manufacture.</param>
        public AddVehicleOutput(long vehicleId, string brand, string model, string licensePlate, int manufactureYear)
        {
            this.VehicleId = vehicleId;
            this.Brand = brand;
            this.Model = model;
            this.LicensePlate = licensePlate;
            this.ManufactureYear = manufactureYear;
        }

        /// <summary>Gets the vehicle identifier.</summary>
        public long VehicleId { get; }

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
