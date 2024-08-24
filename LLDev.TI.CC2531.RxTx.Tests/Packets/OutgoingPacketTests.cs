using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;

namespace LLDev.TI.CC2531.RxTx.Tests.Packets;
public class OutgoingPacketTests
{
    public static readonly TheoryData<OutgoingPacket, byte[]> _theoryData = new()
    {
        // packet size is start byte + len byte + 2 cmd bytes + data + checksum byte
        { new ZbGetDeviceInfoRequest(DeviceInfoType.IeeeAddr), new byte[] { 0xfe, 0x01, 0x26, 0x06, 0x01, 0x20} },
        { new SysResetRequest(ZToolSysResetType.SerialBootloader), new byte[] { 0xfe, 0x01, 0x41, 0x00, 0x01, 0x41 } },
        { new SysResetRequest(ZToolSysResetType.TargetDevice), new byte[] { 0xfe, 0x01, 0x41, 0x00, 0x00, 0x40 } },
        { new SysVersionRequest(), new byte[] { 0xfe, 0x00, 0x21, 0x02, 0x23 } },
        { new UtilLedControlRequest(1, true), new byte[] { 0xfe, 0x02, 0x27, 0x0a, 0x01, 0x01, 0x2f } },
        { new UtilLedControlRequest(1, false), new byte[] { 0xfe, 0x02, 0x27, 0x0a, 0x01, 0x00, 0x2e } },
        { new ZdoMsgCbRegisterRequest(0x1122), new byte[] { 0xfe, 0x02, 0x25, 0x3e, 0x22, 0x11, 0x2a } },
        { new ZbReadConfigRequest(ZToolZbConfigurationId.NvApsAckWaitDuration), new byte[] { 0xfe, 0x01, 0x26, 0x04, 0x44, 0x67 }  },
        { new ZbWriteConfigRequest(ZToolZbConfigurationId.NvApsAckWaitDuration, [0x01, 0x02]), new byte[] { 0xfe, 0x04, 0x26, 0x05, 0x44, 0x02, 0x01, 0x02, 0x62 } },
        { new ZdoStartupFromAppRequest(0x1122), new byte[] { 0xfe, 0x02, 0x25, 0x40, 0x22, 0x11, 0x54 } },
        { new ZdoNodeDescRequest(1, 2), new byte[] { 0xfe, 0x04, 0x25, 0x02, 0x01, 0x00, 0x02, 0x00, 0x20 } },
        { new ZdoNwkDiscoveryRequest(ZToolDiscoveryScanChannelTypes.Channel_11, 10), new byte[] { 0xfe, 0x05, 0x25, 0x26, 0x00, 0x00, 0x08, 0x00, 0x0a, 0x04 } },
        { new SysPingRequest(), new byte[] { 0xfe, 0x00, 0x21, 0x01, 0x20 } },
        { new ZdoActiveEpRequest(0x10, 0x20), new byte[] { 0xfe, 0x04, 0x25, 0x05, 0x00, 0x10, 0x00, 0x20, 0x14 } },
        { new AfRegisterRequest(0x01, 0x0002, 0x0003, 0x04, AfRegisterLatency.FastBeacons, 0x02, [0x0201], 0x02, [0x0403]), new byte[] { 0xfe, 0x0d, 0x24, 0x00, 0x01, 0x02, 0x00, 0x03, 0x00, 0x04, 0x01, 0x02, 0x01, 0x02, 0x02, 0x03, 0x04, 0x28 } },
        { new UtilGetDeviceInfoRequest(), new byte[] { 0xfe, 0x00, 0x27, 0x00, 0x27 } },
        { new ZdoExtRouteDiscoveryRequest(0x0102, 3, 4), new byte[] { 0xfe, 0x05, 0x25, 0x45, 0x02, 0x01, 0x03, 0x04, 0x61 } },
        { new SysGetExtAddrRequest(), new byte[] { 0xfe, 0x00, 0x21, 0x04, 0x25 } },
        { new SysOsalNvLengthRequest(0x0102), new byte[] { 0xfe, 0x02, 0x21, 0x13, 0x02, 0x01, 0x33 } },
        { new SysOsalNvReadRequest(0x0102, 0x03), new byte[] { 0xfe, 0x03, 0x21, 0x08, 0x02, 0x01, 0x03, 0x2a } },
        { new ZdoExtFindGroupRequest(1, 0x0203), new byte[] { 0xfe, 0x03, 0x25, 0x4a, 0x01, 0x03, 0x02, 0x6c } },
        { new ZdoSimpleDescRequest(0x0102, 0x0304, 0x05), new byte[] { 0xfe, 0x05, 0x25, 0x04, 0x02, 0x01, 0x04, 0x03, 0x05, 0x25 } },
        { new ZdoExtNwkInfoRequest(), new byte[] { 0xfe, 0x00, 0x25, 0x50, 0x75 } },
        { new ZdoIeeeAddressRequest(0x0203, IeeeAddressRequestType.Extended, 0x0a), new byte[] { 0xfe, 0x04, 0x25, 0x01, 0x03, 0x02, 0x01, 0x0a, 0x2a } }
    };

    [Theory]
    [MemberData(nameof(_theoryData))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "Do not need it here")]
    public void BuildPacket(OutgoingPacket request, byte[] manualResult)
    {
        // Act.
        var result = request.ToByteArray();

        Assert.Equal(manualResult.Length, result.Length);

        for (var i = 0; i < manualResult.Length; i++)
            Assert.Equal(manualResult[i], result[i]);
    }

    [Fact]
    public void CompareClassHieranchyCount()
    {
        // Arrange.
        const int ExpectedClassesCount = 24;

        // Act.
        var type = typeof(OutgoingPacket);

        var actualCount = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(type.IsAssignableFrom)
            .Count();

        // Assert.
        Assert.Equal(ExpectedClassesCount, actualCount);
    }
}
