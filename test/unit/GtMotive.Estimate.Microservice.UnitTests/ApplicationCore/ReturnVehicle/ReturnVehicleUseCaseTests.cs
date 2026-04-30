using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

using Moq;

using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.ReturnVehicle
{
    /// <summary>
    /// Unit tests for <see cref="ReturnVehicleUseCase"/>.
    /// All dependencies are mocked — no real I/O occurs.
    /// </summary>
    public sealed class ReturnVehicleUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IRentalRepository> _rentalRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IReturnVehicleOutputPort> _outputPortMock;
        private readonly Mock<ITelemetry> _telemetryMock;
        private readonly Mock<IAppLogger<ReturnVehicleUseCase>> _loggerMock;
        private readonly ReturnVehicleUseCase _sut;

        public ReturnVehicleUseCaseTests()
        {
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _rentalRepoMock = new Mock<IRentalRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IReturnVehicleOutputPort>();
            _telemetryMock = new Mock<ITelemetry>();
            _loggerMock = new Mock<IAppLogger<ReturnVehicleUseCase>>();

            _sut = new ReturnVehicleUseCase(
                _vehicleRepoMock.Object,
                _rentalRepoMock.Object,
                _unitOfWorkMock.Object,
                _outputPortMock.Object,
                _telemetryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Execute_WhenVehicleNotFound_CallsNotFoundHandle()
        {
            _vehicleRepoMock
                .Setup(r => r.GetByLicensePlateAsync("UNKNOWN"))
                .ReturnsAsync((Vehicle?)null);

            var input = new ReturnVehicleInput("UNKNOWN");

            await _sut.Execute(input);

            _outputPortMock.Verify(o => o.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _outputPortMock.Verify(o => o.StandardHandle(It.IsAny<ReturnVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenNoActiveRental_CallsNotFoundHandle()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            vehicle.MarkAsRented();

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);
            _rentalRepoMock.Setup(r => r.GetActiveByVehicleIdAsync(vehicle.VehicleId)).ReturnsAsync((Rental?)null);

            var input = new ReturnVehicleInput("1234ABC");

            await _sut.Execute(input);

            _outputPortMock.Verify(o => o.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _outputPortMock.Verify(o => o.StandardHandle(It.IsAny<ReturnVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenValidReturn_CompletesRentalAndInvokesOutputPort()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            vehicle.MarkAsRented();
            var rental = new Rental(vehicle.VehicleId, 1L, DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(4));
            ReturnVehicleOutput? capturedOutput = null;

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);
            _rentalRepoMock.Setup(r => r.GetActiveByVehicleIdAsync(vehicle.VehicleId)).ReturnsAsync(rental);
            _rentalRepoMock.Setup(r => r.UpdateAsync(rental)).Returns(Task.CompletedTask);
            _vehicleRepoMock.Setup(r => r.UpdateAsync(vehicle)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _outputPortMock
                .Setup(o => o.StandardHandle(It.IsAny<ReturnVehicleOutput>()))
                .Callback<ReturnVehicleOutput>(o => capturedOutput = o);

            var input = new ReturnVehicleInput("1234ABC");

            await _sut.Execute(input);

            _rentalRepoMock.Verify(r => r.UpdateAsync(rental), Times.Once);
            _vehicleRepoMock.Verify(r => r.UpdateAsync(vehicle), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _outputPortMock.Verify(o => o.StandardHandle(It.IsAny<ReturnVehicleOutput>()), Times.Once);
            Assert.NotNull(capturedOutput);
            Assert.Equal(rental.RentalId, capturedOutput.RentalId);
            Assert.True(rental.ReturnedDate.HasValue);
            Assert.False(rental.IsActive);
        }

        [Fact]
        public async Task Execute_WhenTransactionFails_RollsBack()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            vehicle.MarkAsRented();
            var rental = new Rental(vehicle.VehicleId, 1L, DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(4));

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);
            _rentalRepoMock.Setup(r => r.GetActiveByVehicleIdAsync(vehicle.VehicleId)).ReturnsAsync(rental);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _rentalRepoMock.Setup(r => r.UpdateAsync(rental)).ThrowsAsync(new InvalidOperationException("DB error"));
            _unitOfWorkMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var input = new ReturnVehicleInput("1234ABC");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Execute(input));

            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
