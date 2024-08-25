﻿using LLDev.TI.CC2531.RxTx.Exceptions;
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
    [InlineData(0x41, 0x80, typeof(SysResetIndCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0xc1, typeof(ZdoEndDeviceAnnceIndCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0xff, typeof(ZdoMsgCbIncomingCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x61, 0x02, typeof(SysVersionResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x3e, typeof(ZdoMsgCbRegisterResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x40, typeof(ZdoStartupFromAppResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x66, 0x04, typeof(ZbReadConfigResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x66, 0x05, typeof(ZbWriteConfigResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x66, 0x06, typeof(ZbGetDeviceInfoResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x67, 0x0a, typeof(UtilLedControlResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x02, typeof(ZdoNodeDescCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0x82, typeof(ZdoNodeDescResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x64, 0x01, typeof(AfDataResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0x80, typeof(ZdoNwkAddrCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0xc0, typeof(ZdoStateChangedIndCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x26, typeof(ZdoNwkDiscoveryResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x61, 0x01, typeof(SysPingResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x05, typeof(ZdoActiveEpResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0x85, typeof(ZdoActiveEpCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x64, 0x00, typeof(AfRegisterResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x67, 0x00, typeof(UtilGetDeviceInfoResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x45, typeof(ZdoExtRouteDiscoveryResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x61, 0x04, typeof(SysGetExtAddrResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x61, 0x13, typeof(SysOsalNvLengthResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x61, 0x08, typeof(SysOsalNvReadResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x4a, typeof(ZdoExtFindGroupResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x04, typeof(ZdoSimpleDescResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0x84, typeof(ZdoSimpleDescCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x50, typeof(ZdoExtNwkInfoResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x44, 0x81, typeof(AfIncomingMessageCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x65, 0x01, typeof(ZdoIeeeAddrResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(0x45, 0x81, typeof(ZdoIeeeAddrCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    public void CreateIncomingPacket(byte msb, byte lsb, Type expectedType, byte[] fakeData)
    {
        // Arrange.
        byte[] headerBytes = [0x01, 0x02, msb, lsb, .. fakeData];

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(headerBytes);

        // Assert.
        Assert.NotNull(result);
        Assert.Equal(expectedType, result.GetType());
    }
}