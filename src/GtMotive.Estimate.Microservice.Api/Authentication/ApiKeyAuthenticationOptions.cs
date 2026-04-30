using Microsoft.AspNetCore.Authentication;

namespace GtMotive.Estimate.Microservice.Api.Authentication
{
    /// <summary>Options for the API Key authentication scheme.</summary>
    public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>Gets or sets the expected API key value.</summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
