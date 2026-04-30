using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAllVehicles;

using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.GetAllVehicles
{
    /// <summary>Presenter for the GetAllVehicles use case.</summary>
    public sealed class GetAllVehiclesPresenter : IOutputPortStandard<GetAllVehiclesOutput>, IWebApiPresenter
    {
        /// <summary>Gets the action result to return from the controller.</summary>
        public IActionResult ActionResult { get; private set; } = new NoContentResult();

        /// <inheritdoc />
        public void StandardHandle(GetAllVehiclesOutput response)
        {
            System.ArgumentNullException.ThrowIfNull(response);
            ActionResult = new OkObjectResult(response.Vehicles);
        }
    }
}
