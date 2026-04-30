using System;
using System.Threading.Tasks;

using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.AddVehicle;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

using Moq;

using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.AddVehicle
{
    /// <summary>
    /// Unit tests for <see cref="AddVehicleUseCase"/>.
    /// All dependencies are mocked — no real I/O occurs.
    /// </summary>
    public sealed class AddVehicleUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IOutputPortStandard<AddVehicleOutput>> _outputPortMock;
        private readonly Mock<ITelemetry> _telemetryMock;
        private readonly Mock<IAppLogger<AddVehicleUseCase>> _loggerMock;
        private readonly AddVehicleUseCase _sut;

        public AddVehicleUseCaseTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _outputPortMock = new Mock<IOutputPortStandard<AddVehicleOutput>>();
            _telemetryMock = new Mock<ITelemetry>();
            _loggerMock = new Mock<IAppLogger<AddVehicleUseCase>>();
            _sut = new AddVehicleUseCase(
                _vehicleRepositoryMock.Object,
                _outputPortMock.Object,
                _telemetryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Execute_WhenManufactureYearExceedsMaxAge_ThrowsDomainException()
        {
            var tooOldYear = DateTime.UtcNow.Year - 6;
            var input = new AddVehicleInput("Toyota", "Corolla", "1234ABC", tooOldYear);

            await Assert.ThrowsAsync<DomainException>(() => _sut.Execute(input));

            _vehicleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
            _outputPortMock.Verify(o => o.StandardHandle(It.IsAny<AddVehicleOutput>()), Times.Never);
            _telemetryMock.Verify(t => t.TrackEvent(It.IsAny<string>(), It.IsAny<System.Collections.Generic.IDictionary<string, string>>(), It.IsAny<System.Collections.Generic.IDictionary<string, double>>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenValidInput_AddsVehicleAndInvokesOutputPort()
        {
            var currentYear = DateTime.UtcNow.Year;
            AddVehicleOutput? capturedOutput = null;

            _vehicleRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
                .Returns(Task.CompletedTask);

            _outputPortMock
                .Setup(o => o.StandardHandle(It.IsAny<AddVehicleOutput>()))
                .Callback<AddVehicleOutput>(output => capturedOutput = output);

            var input = new AddVehicleInput("Toyota", "Corolla", "1234ABC", currentYear);

            await _sut.Execute(input);

            _vehicleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
            Assert.NotNull(capturedOutput);
            Assert.Equal("Toyota", capturedOutput.Brand);
            Assert.Equal("Corolla", capturedOutput.Model);
            Assert.Equal("1234ABC", capturedOutput.LicensePlate);
            Assert.Equal(currentYear, capturedOutput.ManufactureYear);
        }

        [Fact]
        public async Task Execute_WhenValidInput_TracksVehicleAddedEvent()
        {
            var currentYear = DateTime.UtcNow.Year;
            var input = new AddVehicleInput("Toyota", "Corolla", "1234ABC", currentYear);

            _vehicleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);

            await _sut.Execute(input);

            _telemetryMock.Verify(
                t => t.TrackEvent(
                    "VehicleAdded",
                    It.Is<System.Collections.Generic.IDictionary<string, string>>(p =>
                        p["Brand"] == "Toyota" && p["Model"] == "Corolla" && p["LicensePlate"] == "1234ABC"),
                    It.IsAny<System.Collections.Generic.IDictionary<string, double>>()),
                Times.Once);
        }
    }
}
