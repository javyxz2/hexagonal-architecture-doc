using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static readonly ActivitySource ActivitySource = new("GtMotive.Estimate.Renting");

        private readonly IVehicleRepository _vehicleRepository;
        private readonly IOutputPortStandard<AddVehicleOutput> _outputPort;
        private readonly ITelemetry _telemetry;
        private readonly IAppLogger<AddVehicleUseCase> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="outputPort">Output port for standard response.</param>
        /// <param name="telemetry">Telemetry service.</param>
        /// <param name="logger">Logger.</param>
        public AddVehicleUseCase(
            IVehicleRepository vehicleRepository,
            IOutputPortStandard<AddVehicleOutput> outputPort,
            ITelemetry telemetry,
            IAppLogger<AddVehicleUseCase> logger)
        {
            _vehicleRepository = vehicleRepository;
            _outputPort = outputPort;
            _telemetry = telemetry;
            _logger = logger;
        }

        /// <summary>Executes the add vehicle use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(AddVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            using var activity = ActivitySource.StartActivity("vehicle.add");
            activity?.SetTag("vehicle.brand", input.Brand);
            activity?.SetTag("vehicle.model", input.Model);
            activity?.SetTag("vehicle.license_plate", input.LicensePlate);
            activity?.SetTag("vehicle.manufacture_year", input.ManufactureYear);

            try
            {
                int currentYear = DateTime.UtcNow.Year;
                if (input.ManufactureYear < currentYear - MaxVehicleAgeYears)
                {
                    throw new DomainException($"Vehicle manufacture year must not be older than {MaxVehicleAgeYears} years.");
                }

                var vehicle = new Vehicle(input.Brand, input.Model, input.LicensePlate, input.ManufactureYear);
                await _vehicleRepository.AddAsync(vehicle);

                activity?.SetTag("vehicle.id", vehicle.VehicleId);
                activity?.SetStatus(ActivityStatusCode.Ok);

                _logger.LogInformation(
                    "Vehicle added: {Brand} {Model} ({LicensePlate}) year {Year} with id {VehicleId}",
                    vehicle.Brand,
                    vehicle.Model,
                    vehicle.LicensePlate,
                    vehicle.ManufactureYear,
                    vehicle.VehicleId);

                _telemetry.TrackEvent(
                    "VehicleAdded",
                    properties: new Dictionary<string, string>
                    {
                        ["Brand"] = vehicle.Brand,
                        ["Model"] = vehicle.Model,
                        ["LicensePlate"] = vehicle.LicensePlate,
                        ["ManufactureYear"] = vehicle.ManufactureYear.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    },
                    metrics: new Dictionary<string, double> { ["Count"] = 1 });

                _outputPort.StandardHandle(new AddVehicleOutput(
                    vehicle.VehicleId,
                    vehicle.Brand,
                    vehicle.Model,
                    vehicle.LicensePlate,
                    vehicle.ManufactureYear));
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
    }
}
