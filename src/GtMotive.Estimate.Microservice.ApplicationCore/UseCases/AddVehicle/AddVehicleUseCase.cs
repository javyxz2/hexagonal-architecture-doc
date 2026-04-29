using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle
{
    /// <summary>Use case to add a new vehicle to the fleet.</summary>
    public sealed class AddVehicleUseCase : IUseCase<AddVehicleInput>
    {
        private const int MaxVehicleAgeYears = 5;

        private readonly IVehicleRepository _vehicleRepository;
        private readonly IOutputPortStandard<AddVehicleOutput> _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="outputPort">Output port for standard response.</param>
        public AddVehicleUseCase(IVehicleRepository vehicleRepository, IOutputPortStandard<AddVehicleOutput> outputPort)
        {
            _vehicleRepository = vehicleRepository;
            _outputPort = outputPort;
        }

        /// <summary>Executes the add vehicle use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(AddVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            int currentYear = DateTime.UtcNow.Year;
            if (input.ManufactureYear < currentYear - MaxVehicleAgeYears)
            {
                throw new DomainException($"Vehicle manufacture year must not be older than {MaxVehicleAgeYears} years.");
            }

            var vehicle = new Vehicle(input.Brand, input.Model, input.LicensePlate, input.ManufactureYear);
            await _vehicleRepository.AddAsync(vehicle);

            _outputPort.StandardHandle(new AddVehicleOutput(
                vehicle.VehicleId,
                vehicle.Brand,
                vehicle.Model,
                vehicle.LicensePlate,
                vehicle.ManufactureYear));
        }
    }
}
