﻿using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Packets;
public class PacketFactoryTests
{
    private readonly Mock<ILogger<PacketFactory>> _loggerMock = new();
    private readonly Mock<IPacketHeader> _packetHeaderMock = new();

    [Fact]
    public void CreateIncomingPacket_PacketTypeIsUnsupported_LoggerDisabled()
    {
        // Arrange.
        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Error)).Returns(false);

        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ZToolCmdType.Unknown);

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(_packetHeaderMock.Object, []);

        // Assert.
        _packetHeaderMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);

        Assert.Null(result);
    }

    [Fact]
    public void CreateIncomingPacket_PacketTypeIsUnsupported_LoggerEnabled_LoggingPacketException()
    {
        // Arrange.
        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Error)).Returns(true);

        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ZToolCmdType.Unknown);

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(_packetHeaderMock.Object, []);

        // Assert.
        _packetHeaderMock.VerifyAll();

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
    [InlineData(ZToolCmdType.SysResetIndClbk, typeof(SysResetIndCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoEndDeviceAnnceIndClbk, typeof(ZdoEndDeviceAnnceIndCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoMsgCbIncomingClbk, typeof(ZdoMsgCbIncomingCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.SysVersionRsp, typeof(SysVersionResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoMsgCbRegisterRsp, typeof(ZdoMsgCbRegisterResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoStartupFromAppRsp, typeof(ZdoStartupFromAppResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZbReadConfigurationRsp, typeof(ZbReadConfigResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZbWriteConfigurationRsp, typeof(ZbWriteConfigResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZbGetDeviceInfoRsp, typeof(ZbGetDeviceInfoResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.UtilLedControlRsp, typeof(UtilLedControlResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoNodeDescRsp, typeof(ZdoNodeDescResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoNodeDescClbk, typeof(ZdoNodeDescCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.AfDataRsp, typeof(AfDataResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoNwkAddrClbk, typeof(ZdoNwkAddrCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoStateChangedIndClbk, typeof(ZdoStateChangedIndCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoNwkDiscoveryRsp, typeof(ZdoNwkDiscoveryResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.SysPingRsp, typeof(SysPingResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoActiveEpRsp, typeof(ZdoActiveEpResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoActiveEpClbk, typeof(ZdoActiveEpCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.AfRegisterRsp, typeof(AfRegisterResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.UtilGetDeviceInfoRsp, typeof(UtilGetDeviceInfoResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoExtRouteDiscRsp, typeof(ZdoExtRouteDiscoveryResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.SysGetExtAddrRsp, typeof(SysGetExtAddrResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.SysOsalNvLengthRsp, typeof(SysOsalNvLengthResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.SysOsalNvReadRsp, typeof(SysOsalNvReadResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoExtFindGroupRsp, typeof(ZdoExtFindGroupResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoSimpleDescRsp, typeof(ZdoSimpleDescResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoSimpleDescClbk, typeof(ZdoSimpleDescCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoExtNwkInfoRsp, typeof(ZdoExtNwkInfoResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.AfIncomingMsgClbk, typeof(AfIncomingMessageCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoIeeeAddrRsp, typeof(ZdoIeeeAddrResponse), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    [InlineData(ZToolCmdType.ZdoIeeeAddrClbk, typeof(ZdoIeeeAddrCallback), (byte[])[0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]
    public void CreateIncomingPacket(ZToolCmdType cmdType, Type expectedType, byte[] fakeData)
    {
        // Arrange.
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(cmdType);

        byte[] data = [.. fakeData];

        var factory = new PacketFactory(_loggerMock.Object);

        // Act.
        var result = factory.CreateIncomingPacket(_packetHeaderMock.Object, data);

        // Assert.
        _packetHeaderMock.VerifyAll();

        Assert.NotNull(result);
        Assert.Equal(expectedType, result.GetType());
    }
}
