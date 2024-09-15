using LLDev.TI.CC2531.Devices;
using LLDev.TI.CC2531.Enums;
using LLDev.TI.CC2531.Handlers;
using LLDev.TI.CC2531.Models;
using LLDev.TI.CC2531.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.Tests.Handlers;

public class NetworkHandlerTests
{
    private readonly Mock<INetworkCoordinatorService> _networkCoordinatorServiceMock = new();
    private readonly Mock<INetworkDevice> _networkDeviceMock = new();
    private readonly Mock<ILogger<NetworkHandler>> _loggerMock = new();

    [Fact]
    public void AddingAndRemovingDeviceAnnouncedEvent()
    {
        // Arrange. / Act 1.
        var service = new NetworkHandler(null!, _networkDeviceMock.Object, null!);

        // Assert 1.
        _networkDeviceMock.VerifyAdd(m => m.DeviceAnnouncedAsync += It.IsAny<DeviceAnnouncedHandler>(), Times.Never);
        _networkDeviceMock.VerifyRemove(m => m.DeviceAnnouncedAsync -= It.IsAny<DeviceAnnouncedHandler>(), Times.Never);

        // Act 2.
        service.DeviceAnnouncedAsync += OnDeviceAnnounced;

        // Assert 2.
        _networkDeviceMock.VerifyAdd(m => m.DeviceAnnouncedAsync += It.IsAny<DeviceAnnouncedHandler>(), Times.Once);
        _networkDeviceMock.VerifyRemove(m => m.DeviceAnnouncedAsync -= It.IsAny<DeviceAnnouncedHandler>(), Times.Never);

        // Act 3.
        service.DeviceAnnouncedAsync -= OnDeviceAnnounced;

        // Assert 3.
        _networkDeviceMock.VerifyAdd(m => m.DeviceAnnouncedAsync += It.IsAny<DeviceAnnouncedHandler>(), Times.Once);
        _networkDeviceMock.VerifyRemove(m => m.DeviceAnnouncedAsync -= It.IsAny<DeviceAnnouncedHandler>(), Times.Once);

        static Task OnDeviceAnnounced(DeviceAnnounceInfo deviceAnnounceInfo) => Task.CompletedTask;
    }

    [Fact]
    public void AddingAndRemovingDeviceMessageReceivedEvent()
    {
        // Arrange. / Act 1.
        var service = new NetworkHandler(null!, _networkDeviceMock.Object, null!);

        // Assert 1.
        _networkDeviceMock.VerifyAdd(m => m.DeviceMessageReceivedAsync += It.IsAny<EndDeviceMessageReceivedHandler>(), Times.Never);
        _networkDeviceMock.VerifyRemove(m => m.DeviceMessageReceivedAsync -= It.IsAny<EndDeviceMessageReceivedHandler>(), Times.Never);

        // Act 2.
        service.DeviceMessageReceivedAsync += DeviceMessageReceived;

        // Assert 2.
        _networkDeviceMock.VerifyAdd(m => m.DeviceMessageReceivedAsync += It.IsAny<EndDeviceMessageReceivedHandler>(), Times.Once);
        _networkDeviceMock.VerifyRemove(m => m.DeviceMessageReceivedAsync -= It.IsAny<EndDeviceMessageReceivedHandler>(), Times.Never);

        // Act 3.
        service.DeviceMessageReceivedAsync -= DeviceMessageReceived;

        // Assert 3.
        _networkDeviceMock.VerifyAdd(m => m.DeviceMessageReceivedAsync += It.IsAny<EndDeviceMessageReceivedHandler>(), Times.Once);
        _networkDeviceMock.VerifyRemove(m => m.DeviceMessageReceivedAsync -= It.IsAny<EndDeviceMessageReceivedHandler>(), Times.Once);

        static Task DeviceMessageReceived(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message) => Task.CompletedTask;
    }

    [Fact]
    public void NetworkNotStarted()
    {
        // Arrange.
        var deviceInfo = new DeviceInfo(123, 321);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        _networkCoordinatorServiceMock.Setup(m => m.StartupCoordinator()).Returns(deviceInfo);

        var service = new NetworkHandler(_networkCoordinatorServiceMock.Object,
            null!,
            _loggerMock.Object);

        // Assert 1.
        Assert.Null(service.NetworkCoordinatorInfo);

        // Act.
        service.StartZigBeeNetwork();

        // Assert 2.
        _loggerMock.VerifyAll();
        _networkCoordinatorServiceMock.VerifyAll();

        _networkCoordinatorServiceMock.Verify(m => m.RegisterNetworkEndpoints(), Times.Once);

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));

        Assert.NotNull(service.NetworkCoordinatorInfo);
        Assert.Equal(deviceInfo, service.NetworkCoordinatorInfo);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PermitNetworkJoin(bool isJoinPermitted)
    {
        // Arrange.
        var service = new NetworkHandler(_networkCoordinatorServiceMock.Object,
            null!,
            null!);

        // Act.
        service.PermitNetworkJoin(isJoinPermitted);

        // Assert.
        _networkCoordinatorServiceMock.Verify(m => m.SetStatusLedMode(isJoinPermitted), Times.Once);
        _networkCoordinatorServiceMock.Verify(m => m.PermitNetworkJoin(isJoinPermitted), Times.Once);
    }
}
