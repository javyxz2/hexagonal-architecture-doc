namespace GtMotive.Estimate.Microservice.Api.Authentication
{
    /// <summary>Constants for the API Key authentication scheme.</summary>
    public static class ApiKeyAuthenticationDefaults
    {
        /// <summary>The name of the API Key authentication scheme.</summary>
        public const string AuthenticationScheme = "ApiKey";

        /// <summary>The HTTP header name used to send the API key.</summary>
        public const string HeaderName = "X-Api-Key";
    }
}
