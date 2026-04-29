using System;
using System.Text.Json.Serialization;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>HTTP request body for renting a vehicle.</summary>
    public sealed class RentVehicleRequest
    {
        /// <summary>Gets or sets the customer identifier.</summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>Gets or sets the planned start date of the rental.</summary>
        [JsonRequired]
        public DateTime StartDate { get; set; }

        /// <summary>Gets or sets the planned end date of the rental.</summary>
        [JsonRequired]
        public DateTime PlannedEndDate { get; set; }
    }
}
