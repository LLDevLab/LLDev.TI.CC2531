using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;

public class NetworkCoordinatorServiceTests
{
    private readonly Mock<INetworkCoordinator> _networkCoordinatorMock = new();
    private readonly Mock<ITransactionService> _transactionServiceMock = new();
    private readonly Mock<ILogger<NetworkCoordinatorService>> _loggerMock = new();

    [Fact]
    public void StartupCoordinator_StartupFailed_ThrowsNetworkException()
    {
        // Arrange.
        const ushort StartupDelay = 100;

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        _networkCoordinatorMock.Setup(m => m.StartupNetwork(StartupDelay)).Returns(false);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            null!,
            _loggerMock.Object);

        // Act. / Assert.
        var exception = Assert.Throws<NetworkException>(service.StartupCoordinator);

        _loggerMock.VerifyAll();
        _networkCoordinatorMock.VerifyAll();

        _networkCoordinatorMock.Verify(m => m.Initialize(), Times.Once);
        _networkCoordinatorMock.Verify(m => m.PingCoordinatorOrThrow(), Times.Once);
        _networkCoordinatorMock.Verify(m => m.ResetCoordinator(), Times.Once);
        _networkCoordinatorMock.Verify(m => m.GetCoordinatorInfo(), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public void StartupCoordinator()
    {
        // Arrange.
        const ushort StartupDelay = 100;

        var deviceInfo = new DeviceInfo(123, 321);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        _networkCoordinatorMock.Setup(m => m.StartupNetwork(StartupDelay)).Returns(true);
        _networkCoordinatorMock.Setup(m => m.GetCoordinatorInfo()).Returns(deviceInfo);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            null!,
            _loggerMock.Object);

        // Act. 
        var result = service.StartupCoordinator();

        // Assert.
        _loggerMock.VerifyAll();
        _networkCoordinatorMock.VerifyAll();

        _networkCoordinatorMock.Verify(m => m.Initialize(), Times.Once);
        _networkCoordinatorMock.Verify(m => m.PingCoordinatorOrThrow(), Times.Once);
        _networkCoordinatorMock.Verify(m => m.ResetCoordinator(), Times.Once);

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));

        Assert.Equal(deviceInfo, result);
    }

    [Fact]
    public void SetStatusLedMode_Failed_LogWarning()
    {
        // Arrange.
        const byte StatusLedId = 1;
        const bool LedEnable = true;

        _networkCoordinatorMock.Setup(m => m.SetCoordinatorLedMode(StatusLedId, LedEnable)).Returns(false);
        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Warning)).Returns(true);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        service.SetStatusLedMode(LedEnable);

        // Assert.
        _networkCoordinatorMock.VerifyAll();
        _loggerMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public void SetStatusLedMode()
    {
        // Arrange.
        const byte StatusLedId = 1;
        const bool LedEnable = true;

        _networkCoordinatorMock.Setup(m => m.SetCoordinatorLedMode(StatusLedId, LedEnable)).Returns(true);
        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Warning)).Returns(true);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        service.SetStatusLedMode(LedEnable);

        // Assert.
        _networkCoordinatorMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Fact]
    public void RegisterEndpoints_ThrowsNetworkException()
    {
        // Arrange.
        const byte EndpointId1 = 123;
        const byte EndpointId2 = 213;
        const byte EndpointId3 = 221;

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var endpoint1 = new NetworkEndpoint(EndpointId1, 0, 0, 0, AfRegisterLatency.FastBeacons, 0, [], 0, []);
        var endpoint2 = new NetworkEndpoint(EndpointId2, 0, 0, 0, AfRegisterLatency.FastBeacons, 0, [], 0, []);
        var endpoint3 = new NetworkEndpoint(EndpointId3, 0, 0, 0, AfRegisterLatency.FastBeacons, 0, [], 0, []);

        var mockSequence = new MockSequence();

        _networkCoordinatorMock.InSequence(mockSequence).Setup(m => m.GetActiveEndpointIds()).Returns([EndpointId1]);
        _networkCoordinatorMock.InSequence(mockSequence).Setup(m => m.GetSupportedEndpoints()).Returns([endpoint1, endpoint2, endpoint3]);
        _networkCoordinatorMock.InSequence(mockSequence).Setup(m => m.GetActiveEndpointIds()).Returns([EndpointId1, EndpointId3]);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            null!,
            _loggerMock.Object);

        // Act. / Assert.
        var exception = Assert.Throws<NetworkException>(service.RegisterNetworkEndpoints);

        _networkCoordinatorMock.VerifyAll();
        _loggerMock.VerifyAll();

        _networkCoordinatorMock.Verify(m => m.ValidateRegisteredEndpoints(It.IsAny<IEnumerable<byte>>()), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal("Failed to register all endpoints.", exception.Message);
    }

    [Fact]
    public void RegisterEndpoints_Success()
    {
        // Arrange.
        const byte EndpointId1 = 123;
        const byte EndpointId2 = 213;
        const byte EndpointId3 = 221;

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var endpoint1 = new NetworkEndpoint(EndpointId1, 0, 0, 0, AfRegisterLatency.FastBeacons, 0, [], 0, []);
        var endpoint2 = new NetworkEndpoint(EndpointId2, 0, 0, 0, AfRegisterLatency.FastBeacons, 0, [], 0, []);
        var endpoint3 = new NetworkEndpoint(EndpointId3, 0, 0, 0, AfRegisterLatency.FastBeacons, 0, [], 0, []);

        var mockSequence = new MockSequence();

        _networkCoordinatorMock.InSequence(mockSequence).Setup(m => m.GetActiveEndpointIds()).Returns([EndpointId1]);
        _networkCoordinatorMock.InSequence(mockSequence).Setup(m => m.GetSupportedEndpoints()).Returns([endpoint1, endpoint2, endpoint3]);
        _networkCoordinatorMock.InSequence(mockSequence).Setup(m => m.GetActiveEndpointIds()).Returns(
        [
            EndpointId1,
            EndpointId2,
            EndpointId3
        ]);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        service.RegisterNetworkEndpoints();

        // Assert.
        _networkCoordinatorMock.VerifyAll();
        _loggerMock.VerifyAll();

        _networkCoordinatorMock.Verify(m => m.ValidateRegisteredEndpoints(It.Is<IEnumerable<byte>>(ba => ba.Count() == 3
            && ba.All(i => i == EndpointId1 || i == EndpointId2 || i == EndpointId3))), Times.Once);

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PermitNetworkJoin(bool expectedResult)
    {
        // Arrange.
        const byte TransactionId = 123;
        const bool IsPermitted = true;

        _transactionServiceMock.Setup(m => m.GetNextTransactionId()).Returns(TransactionId);

        _networkCoordinatorMock.Setup(m => m.PermitNetworkJoin(TransactionId, IsPermitted)).Returns(expectedResult);

        var service = new NetworkCoordinatorService(_networkCoordinatorMock.Object,
            _transactionServiceMock.Object,
            null!);

        // Act.
        var result = service.PermitNetworkJoin(IsPermitted);

        // Assert.
        _transactionServiceMock.VerifyAll();
        _networkCoordinatorMock.VerifyAll();

        Assert.Equal(expectedResult, result);
    }
}
