using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;

using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    public sealed class RentVehiclePresenter : IRentVehicleOutputPort, IWebApiPresenter
    {
        public IActionResult ActionResult { get; private set; } = new NoContentResult();

        public void StandardHandle(RentVehicleOutput response)
        {
            ActionResult = new OkObjectResult(response);
        }

        public void NotFoundHandle(string message)
        {
            ActionResult = new NotFoundObjectResult(message);
        }
    }
}
