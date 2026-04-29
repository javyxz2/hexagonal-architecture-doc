using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle;

using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.AddVehicle
{
    public sealed class AddVehiclePresenter : IOutputPortStandard<AddVehicleOutput>, IWebApiPresenter
    {
        public IActionResult ActionResult { get; private set; } = new NoContentResult();

        public void StandardHandle(AddVehicleOutput response)
        {
            System.ArgumentNullException.ThrowIfNull(response);
            ActionResult = new CreatedResult($"/api/vehicles/{response.VehicleId}", response);
        }
    }
}
