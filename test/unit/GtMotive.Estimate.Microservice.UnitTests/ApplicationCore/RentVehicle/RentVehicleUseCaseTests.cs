using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

using Moq;

using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.RentVehicle
{
    /// <summary>
    /// Unit tests for <see cref="RentVehicleUseCase"/>.
    /// All dependencies are mocked — no real I/O occurs.
    /// </summary>
    public sealed class RentVehicleUseCaseTests
    {
        private static readonly DateTime StartDate = DateTime.UtcNow.AddDays(1);
        private static readonly DateTime EndDate = DateTime.UtcNow.AddDays(8);

        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IRentalRepository> _rentalRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRentVehicleOutputPort> _outputPortMock;
        private readonly Mock<ITelemetry> _telemetryMock;
        private readonly Mock<IAppLogger<RentVehicleUseCase>> _loggerMock;
        private readonly RentVehicleUseCase _sut;

        public RentVehicleUseCaseTests()
        {
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _rentalRepoMock = new Mock<IRentalRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IRentVehicleOutputPort>();
            _telemetryMock = new Mock<ITelemetry>();
            _loggerMock = new Mock<IAppLogger<RentVehicleUseCase>>();

            _sut = new RentVehicleUseCase(
                _vehicleRepoMock.Object,
                _rentalRepoMock.Object,
                _customerRepoMock.Object,
                _unitOfWorkMock.Object,
                _outputPortMock.Object,
                _telemetryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Execute_WhenEndDateBeforeStartDate_ThrowsDomainException()
        {
            var input = new RentVehicleInput("1234ABC", "Ana García", "12345678A", StartDate, StartDate.AddDays(-1));

            await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(input));

            _vehicleRepoMock.Verify(r => r.GetByLicensePlateAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenVehicleNotFound_CallsNotFoundHandle()
        {
            _vehicleRepoMock
                .Setup(r => r.GetByLicensePlateAsync("UNKNOWN"))
                .ReturnsAsync((Vehicle?)null);

            var input = new RentVehicleInput("UNKNOWN", "Ana García", "12345678A", StartDate, EndDate);

            await _sut.Execute(input);

            _outputPortMock.Verify(o => o.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _outputPortMock.Verify(o => o.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenVehicleNotAvailable_ThrowsDomainException()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            vehicle.MarkAsRented();

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);

            var input = new RentVehicleInput("1234ABC", "Ana García", "12345678A", StartDate, EndDate);

            await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(input));

            _customerRepoMock.Verify(r => r.FindOrCreateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenCustomerAlreadyHasActiveRental_ThrowsDomainException()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            var customer = new Customer("Ana García", "12345678A");

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);
            _customerRepoMock.Setup(r => r.FindOrCreateAsync("Ana García", "12345678A")).ReturnsAsync(customer);
            _rentalRepoMock.Setup(r => r.HasActiveRentalAsync(customer.CustomerId)).ReturnsAsync(true);

            var input = new RentVehicleInput("1234ABC", "Ana García", "12345678A", StartDate, EndDate);

            await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(input));

            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenValidInput_CreatesRentalAndInvokesOutputPort()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            var customer = new Customer("Ana García", "12345678A");
            RentVehicleOutput? capturedOutput = null;

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);
            _customerRepoMock.Setup(r => r.FindOrCreateAsync("Ana García", "12345678A")).ReturnsAsync(customer);
            _rentalRepoMock.Setup(r => r.HasActiveRentalAsync(customer.CustomerId)).ReturnsAsync(false);
            _rentalRepoMock.Setup(r => r.AddAsync(It.IsAny<Rental>())).Returns(Task.CompletedTask);
            _vehicleRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _outputPortMock
                .Setup(o => o.StandardHandle(It.IsAny<RentVehicleOutput>()))
                .Callback<RentVehicleOutput>(o => capturedOutput = o);

            var input = new RentVehicleInput("1234ABC", "Ana García", "12345678A", StartDate, EndDate);

            await _sut.Execute(input);

            _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<Rental>()), Times.Once);
            _vehicleRepoMock.Verify(r => r.UpdateAsync(vehicle), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _outputPortMock.Verify(o => o.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Once);
            Assert.NotNull(capturedOutput);
            Assert.Equal(StartDate, capturedOutput.StartDate);
            Assert.Equal(EndDate, capturedOutput.PlannedEndDate);
        }

        [Fact]
        public async Task Execute_WhenTransactionFails_RollsBack()
        {
            var vehicle = new Vehicle("Toyota", "Corolla", "1234ABC", DateTime.UtcNow.Year);
            var customer = new Customer("Ana García", "12345678A");

            _vehicleRepoMock.Setup(r => r.GetByLicensePlateAsync("1234ABC")).ReturnsAsync(vehicle);
            _customerRepoMock.Setup(r => r.FindOrCreateAsync("Ana García", "12345678A")).ReturnsAsync(customer);
            _rentalRepoMock.Setup(r => r.HasActiveRentalAsync(customer.CustomerId)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _rentalRepoMock.Setup(r => r.AddAsync(It.IsAny<Rental>())).ThrowsAsync(new InvalidOperationException("DB error"));
            _unitOfWorkMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var input = new RentVehicleInput("1234ABC", "Ana García", "12345678A", StartDate, EndDate);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Execute(input));

            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
