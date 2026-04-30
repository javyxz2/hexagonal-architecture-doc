#nullable enable
using System;
using System.Text.Json.Serialization;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>HTTP request body for renting a vehicle.</summary>
    public sealed class RentVehicleRequest
    {
        /// <summary>Gets or sets the full name of the customer.</summary>
        [JsonRequired]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>Gets or sets the DNI of the customer (optional).</summary>
        public string? CustomerDni { get; set; }

        /// <summary>Gets or sets the planned start date of the rental.</summary>
        [JsonRequired]
        public DateTime StartDate { get; set; }

        /// <summary>Gets or sets the planned end date of the rental.</summary>
        [JsonRequired]
        public DateTime PlannedEndDate { get; set; }
    }
}
