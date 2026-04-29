using System.Text.Json.Serialization;

namespace GtMotive.Estimate.Microservice.Api.UseCases.AddVehicle
{
    public sealed class AddVehicleRequest
    {
        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string LicensePlate { get; set; } = string.Empty;

        [JsonRequired]
        public int ManufactureYear { get; set; }
    }
}
