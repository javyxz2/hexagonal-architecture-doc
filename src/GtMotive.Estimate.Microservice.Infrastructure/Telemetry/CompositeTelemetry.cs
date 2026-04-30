using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.Telemetry
{
    /// <summary>
    /// Delegates telemetry calls to multiple <see cref="ITelemetry"/> implementations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CompositeTelemetry : ITelemetry
    {
        private readonly ITelemetry[] _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeTelemetry"/> class.
        /// </summary>
        /// <param name="inner">The telemetry implementations to delegate to.</param>
        public CompositeTelemetry(params ITelemetry[] inner)
        {
            _inner = inner;
        }

        /// <inheritdoc />
        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            foreach (var telemetry in _inner)
            {
                telemetry.TrackEvent(eventName, properties, metrics);
            }
        }

        /// <inheritdoc />
        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            foreach (var telemetry in _inner)
            {
                telemetry.TrackMetric(name, value, properties);
            }
        }
    }
}
