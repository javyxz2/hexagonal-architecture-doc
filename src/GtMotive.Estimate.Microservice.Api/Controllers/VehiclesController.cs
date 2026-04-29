using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Api.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [SuppressMessage("Design", "S107:Methods should not have too many parameters", Justification = "Controller aggregates all vehicle use cases by design.")]
    public class VehiclesController : ControllerBase
    {
        private readonly IUseCase<AddVehicleInput> _addVehicleUseCase;
        private readonly AddVehiclePresenter _addVehiclePresenter;

        private readonly IUseCase<GetAvailableVehiclesInput> _getAvailableVehiclesUseCase;
        private readonly GetAvailableVehiclesPresenter _getAvailableVehiclesPresenter;

        private readonly IUseCase<RentVehicleInput> _rentVehicleUseCase;
        private readonly RentVehiclePresenter _rentVehiclePresenter;

        private readonly IUseCase<ReturnVehicleInput> _returnVehicleUseCase;
        private readonly ReturnVehiclePresenter _returnVehiclePresenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesController"/> class.
        /// </summary>
        /// <param name="addVehicleUseCase">Add vehicle use case.</param>
        /// <param name="addVehiclePresenter">Add vehicle presenter.</param>
        /// <param name="getAvailableVehiclesUseCase">Get available vehicles use case.</param>
        /// <param name="getAvailableVehiclesPresenter">Get available vehicles presenter.</param>
        /// <param name="rentVehicleUseCase">Rent vehicle use case.</param>
        /// <param name="rentVehiclePresenter">Rent vehicle presenter.</param>
        /// <param name="returnVehicleUseCase">Return vehicle use case.</param>
        /// <param name="returnVehiclePresenter">Return vehicle presenter.</param>
        public VehiclesController(
            IUseCase<AddVehicleInput> addVehicleUseCase,
            AddVehiclePresenter addVehiclePresenter,
            IUseCase<GetAvailableVehiclesInput> getAvailableVehiclesUseCase,
            GetAvailableVehiclesPresenter getAvailableVehiclesPresenter,
            IUseCase<RentVehicleInput> rentVehicleUseCase,
            RentVehiclePresenter rentVehiclePresenter,
            IUseCase<ReturnVehicleInput> returnVehicleUseCase,
            ReturnVehiclePresenter returnVehiclePresenter)
        {
            _addVehicleUseCase = addVehicleUseCase;
            _addVehiclePresenter = addVehiclePresenter;
            _getAvailableVehiclesUseCase = getAvailableVehiclesUseCase;
            _getAvailableVehiclesPresenter = getAvailableVehiclesPresenter;
            _rentVehicleUseCase = rentVehicleUseCase;
            _rentVehiclePresenter = rentVehiclePresenter;
            _returnVehicleUseCase = returnVehicleUseCase;
            _returnVehiclePresenter = returnVehiclePresenter;
        }

        /// <summary>Adds a new vehicle to the fleet.</summary>
        /// <param name="request">Vehicle data.</param>
        /// <returns>The created vehicle.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await _addVehicleUseCase.Execute(new AddVehicleInput(request.Brand, request.Model, request.LicensePlate, request.ManufactureYear));
            return _addVehiclePresenter.ActionResult;
        }

        /// <summary>Gets all available vehicles.</summary>
        /// <returns>List of available vehicles.</returns>
        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            await _getAvailableVehiclesUseCase.Execute(new GetAvailableVehiclesInput());
            return _getAvailableVehiclesPresenter.ActionResult;
        }

        /// <summary>Rents a vehicle to a customer.</summary>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="request">Rental request data.</param>
        /// <returns>The rental details.</returns>
        [HttpPost("{vehicleId:long}/rent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RentVehicle(long vehicleId, [FromBody] RentVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await _rentVehicleUseCase.Execute(new RentVehicleInput(vehicleId, request.CustomerId, request.StartDate, request.PlannedEndDate));
            return _rentVehiclePresenter.ActionResult;
        }

        /// <summary>Returns a rented vehicle.</summary>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <returns>The return confirmation.</returns>
        [HttpPost("{vehicleId:long}/return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnVehicle(long vehicleId)
        {
            await _returnVehicleUseCase.Execute(new ReturnVehicleInput(vehicleId));
            return _returnVehiclePresenter.ActionResult;
        }
    }
}
