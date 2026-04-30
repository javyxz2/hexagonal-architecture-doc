using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.Telemetry
{
    /// <summary>
    /// Development-only telemetry implementation that writes events and metrics to the console.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConsoleTelemetry : ITelemetry
    {
        /// <inheritdoc />
        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            var sb = new StringBuilder();
            sb.Append(CultureInfo.InvariantCulture, $"[Telemetry] EVENT: {eventName}");

            if (properties != null)
            {
                foreach (var kv in properties)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" | {kv.Key}={kv.Value}");
                }
            }

            if (metrics != null)
            {
                foreach (var kv in metrics)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" | {kv.Key}={kv.Value}");
                }
            }

            Console.WriteLine(sb.ToString());
        }

        /// <inheritdoc />
        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            var sb = new StringBuilder();
            sb.Append(CultureInfo.InvariantCulture, $"[Telemetry] METRIC: {name}={value}");

            if (properties != null)
            {
                foreach (var kv in properties)
                {
                    sb.Append(CultureInfo.InvariantCulture, $" | {kv.Key}={kv.Value}");
                }
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
