using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

using Prometheus;

namespace GtMotive.Estimate.Microservice.Infrastructure.Telemetry
{
    /// <summary>
    /// Telemetry implementation that exposes events and metrics as Prometheus counters and gauges.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PrometheusTelemetry : ITelemetry
    {
        private static readonly Counter VehiclesAddedTotal = Metrics.CreateCounter(
            "vehicles_added_total",
            "Total number of vehicles added to the fleet.");

        private static readonly Counter VehiclesRentedTotal = Metrics.CreateCounter(
            "vehicles_rented_total",
            "Total number of vehicle rentals.");

        private static readonly Counter VehiclesReturnedTotal = Metrics.CreateCounter(
            "vehicles_returned_total",
            "Total number of completed vehicle rentals.");

        /// <inheritdoc />
        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            switch (eventName)
            {
                case "VehicleAdded":
                    VehiclesAddedTotal.Inc();
                    break;

                case "VehicleRented":
                    VehiclesRentedTotal.Inc();
                    break;

                case "VehicleReturned":
                    VehiclesReturnedTotal.Inc();
                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc />
        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
        }
    }
}
