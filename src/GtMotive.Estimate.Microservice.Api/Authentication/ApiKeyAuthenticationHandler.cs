using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GtMotive.Estimate.Microservice.Api.Authentication
{
    /// <summary>Validates requests using the <c>X-Api-Key</c> header.</summary>
    public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options monitor.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="encoder">The URL encoder.</param>
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        /// <inheritdoc/>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var headerValues))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing {ApiKeyAuthenticationDefaults.HeaderName} header."));
            }

            var providedKey = headerValues.ToString();

            if (string.IsNullOrEmpty(Options.ApiKey) || providedKey != Options.ApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
