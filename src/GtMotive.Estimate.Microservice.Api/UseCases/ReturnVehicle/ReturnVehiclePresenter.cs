using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;

using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    public sealed class ReturnVehiclePresenter : IReturnVehicleOutputPort, IWebApiPresenter
    {
        public IActionResult ActionResult { get; private set; } = new NoContentResult();

        public void StandardHandle(ReturnVehicleOutput response)
        {
            ActionResult = new OkObjectResult(response);
        }

        public void NotFoundHandle(string message)
        {
            ActionResult = new NotFoundObjectResult(message);
        }
    }
}
