using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Api.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs.Vehicles
{
    /// <summary>
    /// Functional (integration) tests for the vehicle use cases.
    /// The host is excluded — services are composed via the real DI container.
    /// Real in-memory repositories are used; no mocks.
    /// </summary>
    public sealed class AddVehicleFunctionalTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        [Fact]
        public async Task AddVehicle_WhenValidInput_VehicleAppearsInAvailableList()
        {
            await Fixture.UsingScope(async serviceProvider =>
            {
                // Arrange
                var addUseCase = serviceProvider.GetRequiredService<IUseCase<AddVehicleInput>>();
                var addPresenter = serviceProvider.GetRequiredService<AddVehiclePresenter>();

                var getUseCase = serviceProvider.GetRequiredService<IUseCase<GetAvailableVehiclesInput>>();
                var getPresenter = serviceProvider.GetRequiredService<GetAvailableVehiclesPresenter>();

                var input = new AddVehicleInput("Honda", "Civic", "5678XYZ", DateTime.UtcNow.Year);

                // Act — add the vehicle
                await addUseCase.Execute(input);

                // Assert — presenter received a 201 Created result
                var createdResult = Assert.IsType<CreatedResult>(addPresenter.ActionResult);
                var addOutput = Assert.IsType<AddVehicleOutput>(createdResult.Value);
                Assert.Equal("Honda", addOutput.Brand);
                Assert.Equal("Civic", addOutput.Model);

                // Act — list available vehicles
                await getUseCase.Execute(new GetAvailableVehiclesInput());

                // Assert — the newly added vehicle is in the list
                var okResult = Assert.IsType<OkObjectResult>(getPresenter.ActionResult);
                var vehicles = (System.Collections.Generic.IReadOnlyList<VehicleDto>)okResult.Value!;
                Assert.Contains(vehicles, v => v.VehicleId == addOutput.VehicleId);
            });
        }
    }
}
