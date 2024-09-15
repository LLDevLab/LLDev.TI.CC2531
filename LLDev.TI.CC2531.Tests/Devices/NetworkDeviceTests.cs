using LLDev.TI.CC2531.Devices;
using LLDev.TI.CC2531.Enums;
using LLDev.TI.CC2531.Models;
using LLDev.TI.CC2531.Packets.Incoming;
using LLDev.TI.CC2531.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.Tests.Devices;
public class NetworkDeviceTests
{
    private readonly Mock<IPacketReceiverTransmitterService> _packetReceiverTransmitterServiceMock = new();
    private readonly Mock<ILogger<NetworkDevice>> _loggerMock = new();

    [Fact]
    public void ConstructAndDispose()
    {
        // Arrange. / Act.
        using (var device = new NetworkDevice(_packetReceiverTransmitterServiceMock.Object,
            null!))
        {
        }

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAdd(m => m.PacketReceived += It.IsAny<PacketReceivedHandler>());
        _packetReceiverTransmitterServiceMock.VerifyRemove(m => m.PacketReceived -= It.IsAny<PacketReceivedHandler>());
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void DeviceAnnounced(bool expectedIsMainPowered, bool expectedIsReceiverOnWhenIdle, bool expectedIsSecure)
    {
        // Arrange.
        const ulong ExpectedIeeeAddr = 123;
        const ushort ExpectedNwkAddr = 213;
        const ushort ExpectedSrcAddr = 231;

        var deviceAnnouncedCount = 0;
        var deviceMessageReceivedCount = 0;

        ulong ieeeAddr = 0;
        var nwkAddr = 0;
        var srcAddr = 0;
        var isMainPowered = false;
        var isReceiverOnWhenIdle = false;
        var isSecure = false;

        var incomingPacketMock = new Mock<IZdoEndDeviceAnnceIndCallback>();

        incomingPacketMock.SetupGet(m => m.IeeeAddr).Returns(ExpectedIeeeAddr);
        incomingPacketMock.SetupGet(m => m.NwkAddr).Returns(ExpectedNwkAddr);
        incomingPacketMock.SetupGet(m => m.SrcAddr).Returns(ExpectedSrcAddr);
        incomingPacketMock.SetupGet(m => m.IsMainPowered).Returns(expectedIsMainPowered);
        incomingPacketMock.SetupGet(m => m.IsReceiverOnWhenIdle).Returns(expectedIsReceiverOnWhenIdle);
        incomingPacketMock.SetupGet(m => m.IsSecure).Returns(expectedIsSecure);

        using var device = new NetworkDevice(_packetReceiverTransmitterServiceMock.Object, null!);

        device.DeviceAnnouncedAsync += OnDeviceAnnounced;
        device.DeviceMessageReceivedAsync += OnDeviceMessageReceived;

        // Act.
        _packetReceiverTransmitterServiceMock.Raise(m => m.PacketReceived += null, incomingPacketMock.Object);

        // Assert.
        incomingPacketMock.VerifyAll();

        _packetReceiverTransmitterServiceMock.VerifyAdd(m => m.PacketReceived += It.IsAny<PacketReceivedHandler>());

        Assert.Equal(1, deviceAnnouncedCount);
        Assert.Equal(0, deviceMessageReceivedCount);
        Assert.Equal(ExpectedIeeeAddr, ieeeAddr);
        Assert.Equal(ExpectedNwkAddr, nwkAddr);
        Assert.Equal(ExpectedSrcAddr, srcAddr);
        Assert.Equal(expectedIsMainPowered, isMainPowered);
        Assert.Equal(expectedIsReceiverOnWhenIdle, isReceiverOnWhenIdle);
        Assert.Equal(expectedIsSecure, isSecure);

        Task OnDeviceAnnounced(DeviceAnnounceInfo deviceAnnounceInfo)
        {
            deviceAnnouncedCount++;

            ieeeAddr = deviceAnnounceInfo.IeeeAddr;
            nwkAddr = deviceAnnounceInfo.NwkAddr;
            srcAddr = deviceAnnounceInfo.SrcAddr;
            isMainPowered = deviceAnnounceInfo.IsMainPowered;
            isReceiverOnWhenIdle = deviceAnnounceInfo.IsReceiverOnWhenIdle;
            isSecure = deviceAnnounceInfo.IsSecure;

            return Task.CompletedTask;
        }

        Task OnDeviceMessageReceived(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message)
        {
            deviceMessageReceivedCount++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void IncomingMessage()
    {
        // Arrange.
        const ushort ExpectedSrcAddr = 123;
        const ZigBeeClusterId ExpectedClusterId = ZigBeeClusterId.PermitJoin;

        var deviceAnnouncedCount = 0;
        var deviceMessageReceivedCount = 0;

        var srcAddr = 0;
        ZigBeeClusterId? localClusterId = null;

        var incomingPacketMock = new Mock<IAfIncomingMessageCallback>();

        incomingPacketMock.SetupGet(m => m.SrcAddr).Returns(ExpectedSrcAddr);
        incomingPacketMock.SetupGet(m => m.ClusterId).Returns(ExpectedClusterId);

        using var device = new NetworkDevice(_packetReceiverTransmitterServiceMock.Object, null!);

        device.DeviceAnnouncedAsync += OnDeviceAnnounced;
        device.DeviceMessageReceivedAsync += OnDeviceMessageReceived;

        // Act.
        _packetReceiverTransmitterServiceMock.Raise(m => m.PacketReceived += null, incomingPacketMock.Object);

        // Assert.
        incomingPacketMock.VerifyAll();

        _packetReceiverTransmitterServiceMock.VerifyAdd(m => m.PacketReceived += It.IsAny<PacketReceivedHandler>());

        Assert.Equal(0, deviceAnnouncedCount);
        Assert.Equal(1, deviceMessageReceivedCount);

        Assert.Equal(ExpectedSrcAddr, srcAddr);
        Assert.Equal(ExpectedClusterId, localClusterId);

        Task OnDeviceAnnounced(DeviceAnnounceInfo deviceAnnounceInfo)
        {
            deviceAnnouncedCount++;
            return Task.CompletedTask;
        }

        Task OnDeviceMessageReceived(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message)
        {
            deviceMessageReceivedCount++;

            srcAddr = nwkAddr;
            localClusterId = clusterId;

            return Task.CompletedTask;
        }
    }

    [Fact]
    public void UnsupportedPacketType()
    {
        // Arrange.
        var deviceAnnouncedCount = 0;
        var deviceMessageReceivedCount = 0;

        var incomingPacketMock = new Mock<IZdoIeeeAddrResponse>();

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        using var device = new NetworkDevice(_packetReceiverTransmitterServiceMock.Object, _loggerMock.Object);

        device.DeviceAnnouncedAsync += OnDeviceAnnounced;
        device.DeviceMessageReceivedAsync += OnDeviceMessageReceived;

        // Act.
        _packetReceiverTransmitterServiceMock.Raise(m => m.PacketReceived += null, incomingPacketMock.Object);

        // Assert.
        _loggerMock.VerifyAll();

        _packetReceiverTransmitterServiceMock.VerifyAdd(m => m.PacketReceived += It.IsAny<PacketReceivedHandler>());

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal(0, deviceAnnouncedCount);
        Assert.Equal(0, deviceMessageReceivedCount);

        Task OnDeviceAnnounced(DeviceAnnounceInfo deviceAnnounceInfo)
        {
            deviceAnnouncedCount++;
            return Task.CompletedTask;
        }

        Task OnDeviceMessageReceived(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message)
        {
            deviceMessageReceivedCount++;
            return Task.CompletedTask;
        }
    }
}
