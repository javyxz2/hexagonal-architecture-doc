using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>Use case to rent a vehicle to a customer.</summary>
    public sealed class RentVehicleUseCase : IUseCase<RentVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IRentVehicleOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="rentalRepository">Rental repository.</param>
        /// <param name="outputPort">Combined output port.</param>
        public RentVehicleUseCase(
            IVehicleRepository vehicleRepository,
            IRentalRepository rentalRepository,
            IRentVehicleOutputPort outputPort)
        {
            _vehicleRepository = vehicleRepository;
            _rentalRepository = rentalRepository;
            _outputPort = outputPort;
        }

        /// <summary>Executes the rent vehicle use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.PlannedEndDate <= input.StartDate)
            {
                throw new DomainException("Planned end date must be after the start date.");
            }

            bool hasActiveRental = await _rentalRepository.HasActiveRentalAsync(input.CustomerId);
            if (hasActiveRental)
            {
                throw new DomainException("Customer already has an active rental.");
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(input.VehicleId);
            if (vehicle == null)
            {
                _outputPort.NotFoundHandle($"Vehicle {input.VehicleId} was not found.");
                return;
            }

            if (!vehicle.IsAvailable)
            {
                throw new DomainException("Vehicle is not available for renting.");
            }

            var rental = new Rental(vehicle.VehicleId, input.CustomerId, input.StartDate, input.PlannedEndDate);
            vehicle.MarkAsRented();

            await _rentalRepository.AddAsync(rental);
            await _vehicleRepository.UpdateAsync(vehicle);

            _outputPort.StandardHandle(new RentVehicleOutput(
                rental.RentalId,
                rental.VehicleId,
                rental.CustomerId,
                rental.StartDate,
                rental.PlannedEndDate));
        }
    }
}
