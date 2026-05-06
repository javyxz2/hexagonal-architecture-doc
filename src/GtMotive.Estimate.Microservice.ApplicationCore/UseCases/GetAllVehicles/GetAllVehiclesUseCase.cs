using System;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAllVehicles
{
    /// <summary>Use case to retrieve all vehicles with their availability status.</summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GetAllVehiclesUseCase"/> class.
    /// </remarks>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="outputPort">Output port for standard response.</param>
    /// <param name="logger">Logger.</param>
    public sealed class GetAllVehiclesUseCase(
        IVehicleRepository vehicleRepository,
        IOutputPortStandard<GetAllVehiclesOutput> outputPort,
        IAppLogger<GetAllVehiclesUseCase> logger) : IUseCase<GetAllVehiclesInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IOutputPortStandard<GetAllVehiclesOutput> _outputPort = outputPort;
        private readonly IAppLogger<GetAllVehiclesUseCase> _logger = logger;

        /// <summary>Executes the get all vehicles use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(GetAllVehiclesInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicles = await _vehicleRepository.GetAllAsync();
            var dtos = vehicles
                .Select(v => new VehicleStatusDto(v.VehicleId, v.Brand, v.Model, v.LicensePlate, v.ManufactureYear, v.IsAvailable))
                .ToList();

            _logger.LogInformation(
                "All vehicles listed: {Total} total, {Available} available, {Rented} rented",
                dtos.Count,
                dtos.Count(v => v.IsAvailable),
                dtos.Count(v => !v.IsAvailable));

            _outputPort.StandardHandle(new GetAllVehiclesOutput(dtos));
        }
    }
}
