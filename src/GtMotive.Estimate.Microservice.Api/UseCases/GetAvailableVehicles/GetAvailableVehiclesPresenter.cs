using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;

using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles
{
    public sealed class GetAvailableVehiclesPresenter : IOutputPortStandard<GetAvailableVehiclesOutput>, IWebApiPresenter
    {
        public IActionResult ActionResult { get; private set; } = new NoContentResult();

        public void StandardHandle(GetAvailableVehiclesOutput response)
        {
            System.ArgumentNullException.ThrowIfNull(response);
            ActionResult = new OkObjectResult(response.Vehicles);
        }
    }
}
