using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Packets;

namespace LLDev.TI.CC2531.RxTx.Tests.Packets;

public class PacketHeaderFactoryTests
{
    [Theory]
    [InlineData((byte[])[1, 2, 3])]
    [InlineData((byte[])[1, 2, 3, 4, 5])]
    public void CreatePacketHeader_NotExpectedHeaderLength_ThrowsPacketException(byte[] data)
    {
        // Arrange.
        const int HeaderDataLength = 4;

        var factory = new PacketHeaderFactory();

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => factory.CreatePacketHeader(data));

        Assert.Equal($"Connot create header. Data length is not equal {HeaderDataLength}.", exception.Message);
    }

    [Fact]
    public void CreatePacketHeader_NotExpectedStartByte_ThrowsPacketException()
    {
        // Arrange.
        byte[] data = [1, 2, 3, 4];

        var factory = new PacketHeaderFactory();

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => factory.CreatePacketHeader(data));

        Assert.Equal($"Cannot create header. Invalid packet start byte '{data[0]}'.", exception.Message);
    }

    #region CreatePacketHeader
    [Theory]
    [InlineData(0x41, 0x80, ZToolCmdType.SysResetIndClbk)]
    [InlineData(0x45, 0xc1, ZToolCmdType.ZdoEndDeviceAnnceIndClbk)]
    [InlineData(0x45, 0xff, ZToolCmdType.ZdoMsgCbIncomingClbk)]
    [InlineData(0x61, 0x02, ZToolCmdType.SysVersionRsp)]
    [InlineData(0x65, 0x3e, ZToolCmdType.ZdoMsgCbRegisterRsp)]
    [InlineData(0x65, 0x40, ZToolCmdType.ZdoStartupFromAppRsp)]
    [InlineData(0x66, 0x04, ZToolCmdType.ZbReadConfigurationRsp)]
    [InlineData(0x66, 0x05, ZToolCmdType.ZbWriteConfigurationRsp)]
    [InlineData(0x66, 0x06, ZToolCmdType.ZbGetDeviceInfoRsp)]
    [InlineData(0x67, 0x0a, ZToolCmdType.UtilLedControlRsp)]
    [InlineData(0x65, 0x02, ZToolCmdType.ZdoNodeDescRsp)]
    [InlineData(0x45, 0x82, ZToolCmdType.ZdoNodeDescClbk)]
    [InlineData(0x64, 0x01, ZToolCmdType.AfDataRsp)]
    [InlineData(0x45, 0x80, ZToolCmdType.ZdoNwkAddrClbk)]
    [InlineData(0x45, 0xc0, ZToolCmdType.ZdoStateChangedIndClbk)]
    [InlineData(0x65, 0x26, ZToolCmdType.ZdoNwkDiscoveryRsp)]
    [InlineData(0x61, 0x01, ZToolCmdType.SysPingRsp)]
    [InlineData(0x65, 0x05, ZToolCmdType.ZdoActiveEpRsp)]
    [InlineData(0x45, 0x85, ZToolCmdType.ZdoActiveEpClbk)]
    [InlineData(0x64, 0x00, ZToolCmdType.AfRegisterRsp)]
    [InlineData(0x67, 0x00, ZToolCmdType.UtilGetDeviceInfoRsp)]
    [InlineData(0x65, 0x45, ZToolCmdType.ZdoExtRouteDiscRsp)]
    [InlineData(0x61, 0x04, ZToolCmdType.SysGetExtAddrRsp)]
    [InlineData(0x61, 0x13, ZToolCmdType.SysOsalNvLengthRsp)]
    [InlineData(0x61, 0x08, ZToolCmdType.SysOsalNvReadRsp)]
    [InlineData(0x65, 0x4a, ZToolCmdType.ZdoExtFindGroupRsp)]
    [InlineData(0x65, 0x04, ZToolCmdType.ZdoSimpleDescRsp)]
    [InlineData(0x45, 0x84, ZToolCmdType.ZdoSimpleDescClbk)]
    [InlineData(0x65, 0x50, ZToolCmdType.ZdoExtNwkInfoRsp)]
    [InlineData(0x44, 0x81, ZToolCmdType.AfIncomingMsgClbk)]
    [InlineData(0x65, 0x01, ZToolCmdType.ZdoIeeeAddrRsp)]
    [InlineData(0x45, 0x81, ZToolCmdType.ZdoIeeeAddrClbk)]
    [InlineData(0x26, 0x06, ZToolCmdType.ZbGetDeviceInfoReq)]
    [InlineData(0x41, 0x00, ZToolCmdType.SysResetReq)]
    [InlineData(0x21, 0x02, ZToolCmdType.SysVersionReq)]
    [InlineData(0x27, 0x0a, ZToolCmdType.UtilLedControlReq)]
    [InlineData(0x25, 0x3e, ZToolCmdType.ZdoMsgCbRegisterReq)]
    [InlineData(0x26, 0x04, ZToolCmdType.ZbReadConfigurationReq)]
    [InlineData(0x26, 0x05, ZToolCmdType.ZbWriteConfigurationReq)]
    [InlineData(0x25, 0x40, ZToolCmdType.ZdoStartupFromAppReq)]
    [InlineData(0x25, 0x02, ZToolCmdType.ZdoNodeDescReq)]
    [InlineData(0x25, 0x26, ZToolCmdType.ZdoNwkDiscoveryReq)]
    [InlineData(0x21, 0x01, ZToolCmdType.SysPingReq)]
    [InlineData(0x25, 0x05, ZToolCmdType.ZdoActiveEpReq)]
    [InlineData(0x24, 0x00, ZToolCmdType.AfRegisterReq)]
    [InlineData(0x27, 0x00, ZToolCmdType.UtilGetDeviceInfoReq)]
    [InlineData(0x25, 0x45, ZToolCmdType.ZdoExtRouteDicsReq)]
    [InlineData(0x21, 0x04, ZToolCmdType.SysGetExtAddrReq)]
    [InlineData(0x21, 0x13, ZToolCmdType.SysOsalNvLengthReq)]
    [InlineData(0x21, 0x08, ZToolCmdType.SysOsalNvReadReq)]
    [InlineData(0x25, 0x4a, ZToolCmdType.ZdoExtFindGroupReq)]
    [InlineData(0x25, 0x04, ZToolCmdType.ZdoSimpleDescReq)]
    [InlineData(0x25, 0x50, ZToolCmdType.ZdoExtNwkInfoReq)]
    [InlineData(0x25, 0x01, ZToolCmdType.ZdoIeeeAddrReq)]
    public void CreatePacketHeader(byte msb, byte lsb, ZToolCmdType expectedType)
    {
        // Arrange.
        const byte StartByte = 0xfe;
        const byte DataLength = 2;

        byte[] data = [StartByte, DataLength, msb, lsb];

        var factory = new PacketHeaderFactory();

        // Act.
        var result = factory.CreatePacketHeader(data);

        // Assert.
        Assert.Equal(StartByte, result.StartByte);
        Assert.Equal(DataLength, result.DataLength);
        Assert.Equal(expectedType, result.CmdType);
    }
    #endregion CreatePacketHeader

    [Fact]
    public void ZToolCmdTypeEnumerationCount()
    {
        // Arrange.
        const int ExpectedCount = 56;

        // Act.
        var result = Enum.GetNames(typeof(ZToolCmdType)).Length;

        // Assert.
        // In case it fails change CreatePacketHeader unit test accordingly
        Assert.Equal(ExpectedCount, result);
    }
}
