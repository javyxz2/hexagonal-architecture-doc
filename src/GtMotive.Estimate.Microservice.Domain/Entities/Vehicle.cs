namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents a vehicle available for renting.
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vehicle"/> class.
        /// </summary>
        /// <param name="brand">Vehicle brand.</param>
        /// <param name="model">Vehicle model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureYear">Year of manufacture.</param>
        public Vehicle(string brand, string model, string licensePlate, int manufactureYear)
        {
            this.Brand = brand;
            this.Model = model;
            this.LicensePlate = licensePlate;
            this.ManufactureYear = manufactureYear;
            this.IsAvailable = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vehicle"/> class.
        /// Required for Entity Framework Core materialization.
        /// </summary>
        protected Vehicle()
        {
        }

        /// <summary>Gets or sets the unique identifier of the vehicle.</summary>
        public long VehicleId { get; protected set; }

        /// <summary>Gets the vehicle brand.</summary>
        public string Brand { get; private set; }

        /// <summary>Gets the vehicle model.</summary>
        public string Model { get; private set; }

        /// <summary>Gets the license plate.</summary>
        public string LicensePlate { get; private set; }

        /// <summary>Gets the year of manufacture.</summary>
        public int ManufactureYear { get; private set; }

        /// <summary>Gets a value indicating whether the vehicle is available for renting.</summary>
        public bool IsAvailable { get; private set; }

        /// <summary>Marks the vehicle as rented.</summary>
        public void MarkAsRented() => this.IsAvailable = false;

        /// <summary>Marks the vehicle as available.</summary>
        public void MarkAsAvailable() => this.IsAvailable = true;
    }
}
