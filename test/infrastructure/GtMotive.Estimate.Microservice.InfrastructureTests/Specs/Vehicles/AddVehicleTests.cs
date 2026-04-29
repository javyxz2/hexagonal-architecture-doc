using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;

using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs.Vehicles
{
    /// <summary>
    /// Infrastructure tests for POST /api/vehicles.
    /// Scope: host-level only — verifies routing, model binding and the error-handling middleware.
    /// Business logic correctness is covered by unit and functional tests.
    /// </summary>
    public sealed class AddVehicleTests : InfrastructureTestBase
    {
        public AddVehicleTests(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task PostVehicle_WhenManufactureYearTooOld_ReturnsBadRequest()
        {
            // Arrange — a request with a year too old reaches the controller but the domain
            // rejects it; BusinessExceptionFilter converts the DomainException to 400.
            using var client = Fixture.Server.CreateClient();
            var body = JsonSerializer.Serialize(new
            {
                Brand = "Toyota",
                Model = "Corolla",
                LicensePlate = "1234ABC",
                ManufactureYear = DateTime.UtcNow.Year - 10,
            });
            using var content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(new Uri("/api/vehicles", UriKind.Relative), content);

            // Assert — 400 proves the host received the request, performed model-binding,
            // executed the controller, and the error filter produced the correct HTTP status.
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostVehicle_WhenBodyIsNotJson_ReturnsBadRequest()
        {
            // Arrange — malformed body triggers model-binding failure at the host level.
            using var client = Fixture.Server.CreateClient();
            using var content = new StringContent("not-a-json", Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync(new Uri("/api/vehicles", UriKind.Relative), content);

            // Assert — ASP.NET Core returns 400 when JSON deserialization fails.
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
