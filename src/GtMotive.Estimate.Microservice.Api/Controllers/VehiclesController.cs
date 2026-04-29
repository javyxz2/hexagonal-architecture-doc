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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await _addVehicleUseCase.Execute(new AddVehicleInput(request.Brand, request.Model, request.LicensePlate, request.ManufactureYear));
            return _addVehiclePresenter.ActionResult;
        }

        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            await _getAvailableVehiclesUseCase.Execute(new GetAvailableVehiclesInput());
            return _getAvailableVehiclesPresenter.ActionResult;
        }

        [HttpPost("{vehicleId:guid}/rent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RentVehicle(Guid vehicleId, [FromBody] RentVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await _rentVehicleUseCase.Execute(new RentVehicleInput(vehicleId, request.CustomerId));
            return _rentVehiclePresenter.ActionResult;
        }

        [HttpPost("{vehicleId:guid}/return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnVehicle(Guid vehicleId)
        {
            await _returnVehicleUseCase.Execute(new ReturnVehicleInput(vehicleId));
            return _returnVehiclePresenter.ActionResult;
        }
    }
}
