namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAllVehicles
{
    /// <summary>DTO representing a vehicle with its availability status.</summary>
    public sealed class VehicleStatusDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleStatusDto"/> class.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureYear">Manufacture year.</param>
        /// <param name="isAvailable">Whether the vehicle is currently available.</param>
        public VehicleStatusDto(long vehicleId, string brand, string model, string licensePlate, int manufactureYear, bool isAvailable)
        {
            this.VehicleId = vehicleId;
            this.Brand = brand;
            this.Model = model;
            this.LicensePlate = licensePlate;
            this.ManufactureYear = manufactureYear;
            this.IsAvailable = isAvailable;
        }

        /// <summary>Gets the vehicle identifier.</summary>
        public long VehicleId { get; }

        /// <summary>Gets the brand.</summary>
        public string Brand { get; }

        /// <summary>Gets the model.</summary>
        public string Model { get; }

        /// <summary>Gets the license plate.</summary>
        public string LicensePlate { get; }

        /// <summary>Gets the manufacture year.</summary>
        public int ManufactureYear { get; }

        /// <summary>Gets a value indicating whether the vehicle is available.</summary>
        public bool IsAvailable { get; }
    }
}
