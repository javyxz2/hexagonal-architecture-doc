using System;
using System.Linq;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetAvailableVehicles
{
    /// <summary>Use case to retrieve all available vehicles.</summary>
    public sealed class GetAvailableVehiclesUseCase : IUseCase<GetAvailableVehiclesInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IOutputPortStandard<GetAvailableVehiclesOutput> _outputPort;
        private readonly IAppLogger<GetAvailableVehiclesUseCase> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableVehiclesUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="outputPort">Output port for standard response.</param>
        /// <param name="logger">Logger.</param>
        public GetAvailableVehiclesUseCase(
            IVehicleRepository vehicleRepository,
            IOutputPortStandard<GetAvailableVehiclesOutput> outputPort,
            IAppLogger<GetAvailableVehiclesUseCase> logger)
        {
            _vehicleRepository = vehicleRepository;
            _outputPort = outputPort;
            _logger = logger;
        }

        /// <summary>Executes the get available vehicles use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(GetAvailableVehiclesInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicles = await _vehicleRepository.GetAvailableAsync();
            var dtos = vehicles
                .OrderBy(v => v.VehicleId)
                .Select(v => new VehicleDto(v.VehicleId, v.Brand, v.Model, v.LicensePlate, v.ManufactureYear))
                .ToList();

            _logger.LogInformation(
                "Available vehicles listed: {Count} available",
                dtos.Count);

            _outputPort.StandardHandle(new GetAvailableVehiclesOutput(dtos));
        }
    }
}
