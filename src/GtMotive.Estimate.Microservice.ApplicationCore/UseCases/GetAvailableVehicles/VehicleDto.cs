namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles
{
    /// <summary>DTO representing a vehicle.</summary>
    public sealed class VehicleDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleDto"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureYear">Year of manufacture.</param>
        public VehicleDto(long vehicleId, string brand, string model, string licensePlate, int manufactureYear)
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
