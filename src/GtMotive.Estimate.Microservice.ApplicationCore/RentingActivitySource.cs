using System.Diagnostics;

namespace GtMotive.Estimate.Microservice.ApplicationCore
{
    /// <summary>Shared <see cref="ActivitySource"/> for all renting use cases.</summary>
    internal static class RentingActivitySource
    {
        /// <summary>The single <see cref="ActivitySource"/> instance for the renting domain.</summary>
        internal static readonly ActivitySource Instance = new("GtMotive.Estimate.Renting");
    }
}
