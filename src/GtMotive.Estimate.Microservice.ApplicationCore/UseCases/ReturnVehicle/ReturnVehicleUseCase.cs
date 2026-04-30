using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>Use case to return a rented vehicle.</summary>
    public sealed class ReturnVehicleUseCase : IUseCase<ReturnVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IReturnVehicleOutputPort _outputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVehicleUseCase"/> class.
        /// </summary>
        /// <param name="vehicleRepository">Vehicle repository.</param>
        /// <param name="rentalRepository">Rental repository.</param>
        /// <param name="outputPort">Combined output port.</param>
        public ReturnVehicleUseCase(
            IVehicleRepository vehicleRepository,
            IRentalRepository rentalRepository,
            IReturnVehicleOutputPort outputPort)
        {
            _vehicleRepository = vehicleRepository;
            _rentalRepository = rentalRepository;
            _outputPort = outputPort;
        }

        /// <summary>Executes the return vehicle use case.</summary>
        /// <param name="input">Input data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var rental = await _rentalRepository.GetActiveByVehicleIdAsync(input.VehicleId);
            if (rental == null)
            {
                _outputPort.NotFoundHandle($"No active rental found for vehicle {input.VehicleId}.");
                return;
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(input.VehicleId);
            if (vehicle == null)
            {
                _outputPort.NotFoundHandle($"Vehicle {input.VehicleId} was not found.");
                return;
            }

            rental.Complete();
            vehicle.MarkAsAvailable();

            await _rentalRepository.UpdateAsync(rental);
            await _vehicleRepository.UpdateAsync(vehicle);

            _outputPort.StandardHandle(new ReturnVehicleOutput(rental.RentalId, rental.ReturnedDate!.Value));
        }
    }
}
