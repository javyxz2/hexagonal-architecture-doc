using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>Use case to return a rented vehicle.</summary>
    public sealed class ReturnVehicleUseCase : IUseCase<ReturnVehicleInput>
    {
        private static readonly ActivitySource ActivitySource = new("GtMotive.Estimate.Renting");

        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IReturnVehicleOutputPort _outputPort;
        private readonly ITelemetry _telemetry;
        private readonly IAppLogger<ReturnVehicleUseCase> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="rentalRepository">Rental repository.</param>
        /// <param name="outputPort">Combined output port.</param>
        /// <param name="telemetry">Telemetry service.</param>
        /// <param name="logger">Logger.</param>
        public ReturnVehicleUseCase(
            IVehicleRepository vehicleRepository,
            IRentalRepository rentalRepository,
            IReturnVehicleOutputPort outputPort,
            ITelemetry telemetry,
            IAppLogger<ReturnVehicleUseCase> logger)
        {
            _vehicleRepository = vehicleRepository;
            _rentalRepository = rentalRepository;
            _outputPort = outputPort;
            _telemetry = telemetry;
            _logger = logger;
        }

        /// <summary>Executes the return vehicle use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            using var activity = ActivitySource.StartActivity("vehicle.return");
            activity?.SetTag("vehicle.license_plate", input.LicensePlate);

            try
            {
                var vehicle = await _vehicleRepository.GetByLicensePlateAsync(input.LicensePlate);
                if (vehicle == null)
                {
                    _outputPort.NotFoundHandle($"Vehicle with license plate '{input.LicensePlate}' was not found.");
                    return;
                }

                var rental = await _rentalRepository.GetActiveByVehicleIdAsync(vehicle.VehicleId);
                if (rental == null)
                {
                    _outputPort.NotFoundHandle($"No active rental found for vehicle '{input.LicensePlate}'.");
                    return;
                }

                rental.Complete();
                vehicle.MarkAsAvailable();

                await _rentalRepository.UpdateAsync(rental);
                await _vehicleRepository.UpdateAsync(vehicle);

                activity?.SetTag("rental.id", rental.RentalId);
                activity?.SetTag("rental.customer_id", rental.CustomerId);
                activity?.SetTag("rental.returned_date", rental.ReturnedDate);
                activity?.SetStatus(ActivityStatusCode.Ok);

                _logger.LogInformation(
                    "Vehicle {VehicleId} returned by customer {CustomerId} on {ReturnedDate:yyyy-MM-dd} (rentalId: {RentalId})",
                    rental.VehicleId,
                    rental.CustomerId,
                    rental.ReturnedDate,
                    rental.RentalId);

                _telemetry.TrackEvent(
                    "VehicleReturned",
                    properties: new Dictionary<string, string>
                    {
                        ["VehicleId"] = rental.VehicleId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        ["CustomerId"] = rental.CustomerId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        ["RentalId"] = rental.RentalId.ToString(),
                        ["ReturnedDate"] = rental.ReturnedDate?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    },
                    metrics: new Dictionary<string, double> { ["Count"] = 1 });

                _outputPort.StandardHandle(new ReturnVehicleOutput(rental.RentalId, rental.ReturnedDate!.Value));
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
    }
}
