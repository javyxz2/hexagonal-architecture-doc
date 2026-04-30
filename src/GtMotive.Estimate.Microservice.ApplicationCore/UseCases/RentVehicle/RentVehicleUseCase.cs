#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>Use case to rent a vehicle to a customer.</summary>
    public sealed class RentVehicleUseCase : IUseCase<RentVehicleInput>
    {
        private static readonly ActivitySource ActivitySource = new("GtMotive.Estimate.Renting");

        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRentVehicleOutputPort _outputPort;
        private readonly ITelemetry _telemetry;
        private readonly IAppLogger<RentVehicleUseCase> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="rentalRepository">Rental repository.</param>
        /// <param name="customerRepository">Customer repository.</param>
        /// <param name="unitOfWork">Unit of work for transactional control.</param>
        /// <param name="outputPort">Combined output port.</param>
        /// <param name="telemetry">Telemetry service.</param>
        /// <param name="logger">Logger.</param>
        public RentVehicleUseCase(
            IVehicleRepository vehicleRepository,
            IRentalRepository rentalRepository,
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            IRentVehicleOutputPort outputPort,
            ITelemetry telemetry,
            IAppLogger<RentVehicleUseCase> logger)
        {
            _vehicleRepository = vehicleRepository;
            _rentalRepository = rentalRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _outputPort = outputPort;
            _telemetry = telemetry;
            _logger = logger;
        }

        /// <summary>Executes the rent vehicle use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            using var activity = ActivitySource.StartActivity("vehicle.rent");
            activity?.SetTag("vehicle.license_plate", input.LicensePlate);
            activity?.SetTag("rental.customer_name", input.CustomerName);
            activity?.SetTag("rental.start_date", input.StartDate);
            activity?.SetTag("rental.planned_end_date", input.PlannedEndDate);

            try
            {
                if (input.PlannedEndDate <= input.StartDate)
                {
                    throw new DomainException("Planned end date must be after the start date.");
                }

                var customer = await FindOrCreateCustomerAsync(input.CustomerName, input.CustomerDni);
                bool hasActiveRental = await _rentalRepository.HasActiveRentalAsync(customer.CustomerId);
                if (hasActiveRental)
                {
                    throw new DomainException("Customer already has an active rental.");
                }

                var vehicle = await _vehicleRepository.GetByLicensePlateAsync(input.LicensePlate);
                if (vehicle == null)
                {
                    _outputPort.NotFoundHandle($"Vehicle with license plate '{input.LicensePlate}' was not found.");
                    return;
                }

                if (!vehicle.IsAvailable)
                {
                    throw new DomainException("Vehicle is not available for renting.");
                }

                var rental = new Rental(vehicle.VehicleId, customer.CustomerId, input.StartDate, input.PlannedEndDate);
                vehicle.MarkAsRented();

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _rentalRepository.AddAsync(rental);
                    await _vehicleRepository.UpdateAsync(vehicle);
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }

                activity?.SetTag("rental.id", rental.RentalId);
                activity?.SetTag("rental.customer_id", customer.CustomerId);
                activity?.SetStatus(ActivityStatusCode.Ok);

                _logger.LogInformation(
                    "Vehicle {LicensePlate} rented by customer {CustomerId} ({CustomerName}) from {StartDate:yyyy-MM-dd} to {PlannedEndDate:yyyy-MM-dd} (rentalId: {RentalId})",
                    input.LicensePlate,
                    customer.CustomerId,
                    customer.CustomerName,
                    rental.StartDate,
                    rental.PlannedEndDate,
                    rental.RentalId);

                _telemetry.TrackEvent(
                    "VehicleRented",
                    properties: new Dictionary<string, string>
                    {
                        ["VehicleId"] = rental.VehicleId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        ["LicensePlate"] = input.LicensePlate,
                        ["CustomerId"] = customer.CustomerId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        ["CustomerName"] = customer.CustomerName,
                        ["StartDate"] = rental.StartDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                        ["PlannedEndDate"] = rental.PlannedEndDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                        ["RentalId"] = rental.RentalId.ToString(),
                    },
                    metrics: new Dictionary<string, double> { ["Count"] = 1 });

                _outputPort.StandardHandle(new RentVehicleOutput(
                    rental.RentalId,
                    rental.VehicleId,
                    customer.CustomerId,
                    rental.StartDate,
                    rental.PlannedEndDate));
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }

        private async Task<Customer> FindOrCreateCustomerAsync(string customerName, string customerDni)
        {
            var customer = await _customerRepository.FindOrCreateAsync(customerName, customerDni);

            _logger.LogInformation(
                "Customer resolved: {CustomerName} (DNI: {CustomerDni}, Id: {CustomerId})",
                customer.CustomerName,
                customer.CustomerDni,
                customer.CustomerId);

            return customer;
        }
    }
}
