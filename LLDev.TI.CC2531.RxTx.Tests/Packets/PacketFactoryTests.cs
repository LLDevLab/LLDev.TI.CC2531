using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Packets;
public class PacketFactoryTests
{
    private readonly Mock<ILogger<PacketFactory>> _loggerMock = new();

    [Fact]
    public void CreateIncomingPacket_PacketLengthIsTooLow_LoggingPacketException()
    {
        // Arrange.
        const int HeaderLen = 4;

        var packet = new byte[] { 1, 2, 3 };

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(packet);

        // Assert.
        _loggerMock.Verify(x => x.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.Is<PacketException>(e => e.Message == $"Cannot create packet header. Packet length is less that {HeaderLen} bytes"),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Null(result);
    }

    [Fact]
    public void CreateIncomingPacket_PacketTypeIsUnsupported_LoggingPacketException()
    {
        // Arrange.
        var packet = new byte[] { 1, 2, 3, 4 };

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(packet);

        // Assert.
        _loggerMock.Verify(x => x.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.Is<PacketException>(e => e.Message == "Unsupported packet type"),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Null(result);
    }

    [Fact]
    public void CompareClassHieranchyCount()
    {
        // Arrange.
        const int ExpectedClassesCount = 33;

        // Act.
        var type = typeof(IncomingPacket);

        var actualCount = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(type.IsAssignableFrom)
            .Count();

        // Assert.
        Assert.Equal(ExpectedClassesCount, actualCount);
    }

    [Theory]
    [InlineData(0x41, 0x80, typeof(SysResetIndCallback))]
    [InlineData(0x45, 0x1c, typeof(ZdoEndDeviceAnnceIndCallback))]
    [InlineData(0x45, 0xff, typeof(ZdoMsgCbIncomingCallback))]
    [InlineData(0x61, 0x02, typeof(SysVersionResponse))]
    [InlineData(0x65, 0x3e, typeof(ZdoMsgCbRegisterResponse))]
    [InlineData(0x65, 0x40, typeof(ZdoStartupFromAppResponse))]
    [InlineData(0x66, 0x04, typeof(ZbReadConfigResponse))]
    [InlineData(0x66, 0x05, typeof(ZbWriteConfigResponse))]
    [InlineData(0x66, 0x06, typeof(ZbGetDeviceInfoResponse))]
    [InlineData(0x67, 0x0a, typeof(UtilLedControlResponse))]
    [InlineData(0x65, 0x02, typeof(ZdoNodeDescCallback))]
    [InlineData(0x45, 0x82, typeof(ZdoNodeDescResponse))]
    [InlineData(0x64, 0x01, typeof(AfDataResponse))]
    [InlineData(0x45, 0x80, typeof(ZdoNwkAddrCallback))]
    [InlineData(0x45, 0xc0, typeof(ZdoStateChangedIndCallback))]
    [InlineData(0x65, 0x26, typeof(ZdoNwkDiscoveryResponse))]
    [InlineData(0x61, 0x01, typeof(SysPingResponse))]
    [InlineData(0x65, 0x05, typeof(ZdoActiveEpResponse))]
    [InlineData(0x45, 0x85, typeof(ZdoActiveEpCallback))]
    [InlineData(0x64, 0x00, typeof(AfRegisterResponse))]
    [InlineData(0x67, 0x00, typeof(UtilGetDeviceInfoResponse))]
    [InlineData(0x65, 0x45, typeof(ZdoExtRouteDiscoveryResponse))]
    [InlineData(0x61, 0x04, typeof(SysGetExtAddrResponse))]
    [InlineData(0x61, 0x13, typeof(SysOsalNvLengthResponse))]
    [InlineData(0x61, 0x08, typeof(SysOsalNvReadResponse))]
    [InlineData(0x65, 0x4a, typeof(ZdoExtFindGroupResponse))]
    [InlineData(0x65, 0x04, typeof(ZdoSimpleDescResponse))]
    [InlineData(0x45, 0x84, typeof(ZdoSimpleDescCallback))]
    [InlineData(0x65, 0x50, typeof(ZdoExtNwkInfoResponse))]
    [InlineData(0x44, 0x81, typeof(AfIncomingMessageCallback))]
    [InlineData(0x65, 0x01, typeof(ZdoIeeeAddrResponse))]
    [InlineData(0x45, 0x81, typeof(ZdoIeeeAddrCallback))]
    public void CreateIncomingPacket(byte msb, byte lsb, Type expectedType)
    {
        // Arrange.
        var headerBytes = new byte[] { 0x01, 0x02, msb, lsb, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0f, 0x10 };

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(headerBytes);

        // Assert.
        Assert.NotNull(result);
        Assert.Equal(expectedType, result.GetType());
    }
}
