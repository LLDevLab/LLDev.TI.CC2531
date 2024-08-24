using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Packets;

namespace LLDev.TI.CC2531.RxTx.Tests.Packets;
public class PacketHeaderTests
{
    [Fact]
    public void PacketLengthIsTooLow_ThrowsPacketException()
    {
        // Arrange.
        const int HeaderLen = 4;

        var packet = new byte[] { 1, 2, 3 };

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => new PacketHeader(packet));

        Assert.Equal($"Cannot create packet header. Packet length is less that {HeaderLen} bytes", exception.Message);
    }

    [Fact]
    public void GetStartByte()
    {
        // Arrange.
        const byte StartByte = 1;

        var arr = new byte[] { StartByte, 2, 3, 4, 5, 6 };

        var header = new PacketHeader(arr);

        // Act.
        var result = header.StartByte;

        // Assert.
        Assert.Equal(StartByte, result);
    }

    [Fact]
    public void GetDataLength()
    {
        // Arrange.
        const byte DataLength = 2;

        var arr = new byte[] { 1, DataLength, 3, 4, 5, 6 };

        var header = new PacketHeader(arr);

        // Act.
        var result = header.DataLength;

        // Assert.
        Assert.Equal(DataLength, result);
    }

    [Fact]
    public void KnownCmdTypeCountNotChanged()
    {
        // Arrange.
        const int ExpectedCount = 56;

        // Act.
        var actualCount = Enum.GetNames(typeof(ZToolCmdType)).Length;

        // Assert.

        // In case of failure, please change GetCmdType test accordingly
        Assert.Equal(ExpectedCount, actualCount);
    }

    #region GetCmdType
    [Theory]
    [InlineData(0x01, 0x02, ZToolCmdType.Unknown)]
    [InlineData(0x10, 0x20, ZToolCmdType.Unknown)]
    [InlineData(0x21, 0x01, ZToolCmdType.SysPingReq)]
    [InlineData(0x21, 0x02, ZToolCmdType.SysVersionReq)]
    [InlineData(0x21, 0x04, ZToolCmdType.SysGetExtAddrReq)]
    [InlineData(0x21, 0x08, ZToolCmdType.SysOsalNvReadReq)]
    [InlineData(0x21, 0x13, ZToolCmdType.SysOsalNvLengthReq)]
    [InlineData(0x24, 0x00, ZToolCmdType.AfRegisterReq)]
    [InlineData(0x24, 0x01, ZToolCmdType.AfDataReq)]
    [InlineData(0x25, 0x01, ZToolCmdType.ZdoIeeeAddrReq)]
    [InlineData(0x25, 0x02, ZToolCmdType.ZdoNodeDescReq)]
    [InlineData(0x25, 0x04, ZToolCmdType.ZdoSimpleDescReq)]
    [InlineData(0x25, 0x05, ZToolCmdType.ZdoActiveEpReq)]
    [InlineData(0x25, 0x26, ZToolCmdType.ZdoNwkDiscoveryReq)]
    [InlineData(0x25, 0x3e, ZToolCmdType.ZdoMsgCbRegisterReq)]
    [InlineData(0x25, 0x40, ZToolCmdType.ZdoStartupFromAppReq)]
    [InlineData(0x25, 0x45, ZToolCmdType.ZdoExtRouteDicsReq)]
    [InlineData(0x25, 0x4a, ZToolCmdType.ZdoExtFindGroupReq)]
    [InlineData(0x25, 0x50, ZToolCmdType.ZdoExtNwkInfoReq)]
    [InlineData(0x26, 0x04, ZToolCmdType.ZbReadConfigurationReq)]
    [InlineData(0x26, 0x05, ZToolCmdType.ZbWriteConfigurationReq)]
    [InlineData(0x26, 0x06, ZToolCmdType.ZbGetDeviceInfoReq)]
    [InlineData(0x27, 0x00, ZToolCmdType.UtilGetDeviceInfoReq)]
    [InlineData(0x27, 0x0a, ZToolCmdType.UtilLedControlReq)]
    [InlineData(0x41, 0x00, ZToolCmdType.SysResetReq)]
    [InlineData(0x41, 0x80, ZToolCmdType.SysResetIndClbk)]
    [InlineData(0x44, 0x81, ZToolCmdType.AfIncomingMsgClbk)]
    [InlineData(0x45, 0x80, ZToolCmdType.ZdoNwkAddrClbk)]
    [InlineData(0x45, 0x81, ZToolCmdType.ZdoIeeeAddrClbk)]
    [InlineData(0x45, 0x82, ZToolCmdType.ZdoNodeDescClbk)]
    [InlineData(0x45, 0x84, ZToolCmdType.ZdoSimpleDescClbk)]
    [InlineData(0x45, 0x85, ZToolCmdType.ZdoActiveEpClbk)]
    [InlineData(0x45, 0xc0, ZToolCmdType.ZdoStateChangedIndClbk)]
    [InlineData(0x45, 0xc1, ZToolCmdType.ZdoEndDeviceAnnceIndClbk)]
    [InlineData(0x45, 0xff, ZToolCmdType.ZdoMsgCbIncomingClbk)]
    [InlineData(0x61, 0x01, ZToolCmdType.SysPingRsp)]
    [InlineData(0x61, 0x02, ZToolCmdType.SysVersionRsp)]
    [InlineData(0x61, 0x04, ZToolCmdType.SysGetExtAddrRsp)]
    [InlineData(0x61, 0x08, ZToolCmdType.SysOsalNvReadRsp)]
    [InlineData(0x61, 0x13, ZToolCmdType.SysOsalNvLengthRsp)]
    [InlineData(0x64, 0x00, ZToolCmdType.AfRegisterRsp)]
    [InlineData(0x64, 0x01, ZToolCmdType.AfDataRsp)]
    [InlineData(0x65, 0x01, ZToolCmdType.ZdoIeeeAddrRsp)]
    [InlineData(0x65, 0x02, ZToolCmdType.ZdoNodeDescRsp)]
    [InlineData(0x65, 0x04, ZToolCmdType.ZdoSimpleDescRsp)]
    [InlineData(0x65, 0x05, ZToolCmdType.ZdoActiveEpRsp)]
    [InlineData(0x65, 0x26, ZToolCmdType.ZdoNwkDiscoveryRsp)]
    [InlineData(0x65, 0x3e, ZToolCmdType.ZdoMsgCbRegisterRsp)]
    [InlineData(0x65, 0x40, ZToolCmdType.ZdoStartupFromAppRsp)]
    [InlineData(0x65, 0x45, ZToolCmdType.ZdoExtRouteDiscRsp)]
    [InlineData(0x65, 0x4a, ZToolCmdType.ZdoExtFindGroupRsp)]
    [InlineData(0x65, 0x50, ZToolCmdType.ZdoExtNwkInfoRsp)]
    [InlineData(0x66, 0x04, ZToolCmdType.ZbReadConfigurationRsp)]
    [InlineData(0x66, 0x05, ZToolCmdType.ZbWriteConfigurationRsp)]
    [InlineData(0x66, 0x06, ZToolCmdType.ZbGetDeviceInfoRsp)]
    [InlineData(0x67, 0x00, ZToolCmdType.UtilGetDeviceInfoRsp)]
    [InlineData(0x67, 0x0a, ZToolCmdType.UtilLedControlRsp)]
    public void GetCmdType(byte lsb, byte msb, ZToolCmdType expectedCmdType)
    {
        // Arrange.
        var arr = new byte[] { 1, 2, lsb, msb, 5, 6 };

        var header = new PacketHeader(arr);

        // Act.
        var result = header.CmdType;

        // Assert.
        Assert.Equal(expectedCmdType, result);
    }
    #endregion GetCmdType

    [Fact]
    public void ToByteArray()
    {
        // Arrange.
        var arr = new byte[] { 1, 2, 3, 4, 5, 6 };

        var header = new PacketHeader(arr);

        // Act.
        var result = header.ToByteArray();

        // Assert.
        Assert.Collection(result,
            item => Assert.Equal(arr[0], item),
            item => Assert.Equal(arr[1], item),
            item => Assert.Equal(arr[2], item),
            item => Assert.Equal(arr[3], item));
    }
}
