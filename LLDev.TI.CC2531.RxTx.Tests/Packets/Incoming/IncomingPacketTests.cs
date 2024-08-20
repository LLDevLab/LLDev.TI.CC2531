using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;

namespace LLDev.TI.CC2531.RxTx.Tests.Packets.Incoming;
public class IncomingPacketTests
{
    // TODO: Does all incoming packets tested?

    private const int StartByte = 0xfe;

    private readonly Mock<IPacketHeader> _packetHeaderMock = new();

    [Fact]
    public void SysVersionResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x05;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.SysVersionRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x61, 0x02]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x67 };
        var response = new SysVersionResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x01, response.TransportRev);
        Assert.Equal(0x02, response.ProductId);
        Assert.Equal(0x03, response.MajorRel);
        Assert.Equal(0x04, response.MinorRel);
        Assert.Equal(0x05, response.MaintRel);
    }

    [Fact]
    public void ZbReadConfigResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x04;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZbReadConfigurationRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x66, 0x04]);

        var data = new byte[] { 0x01, 0x03, 0x01, 0x04, 0x61 };

        var response = new ZbReadConfigResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, response.Status);
        Assert.Equal(ZToolZbConfigurationId.NvStartupOption, response.ConfigId);
        Assert.Equal(0x01, response.ConfigValueLen);

        var configVal = response.ConfigValue;

        for (var i = 0; i < configVal.Length; i++)
            Assert.Equal(data[i + 3], configVal[i]);
    }

    [Fact]
    public void ZbWriteConfigResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZbWriteConfigurationRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x66, 0x05]);

        var data = new byte[] { 0x01, 0x63 };
        var response = new ZbWriteConfigResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, response.Status);
    }

    [Fact]
    public void ZdoStartupFromAppResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoStartupFromAppRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x40]);

        var data = new byte[] { 0x01, 0x25 };
        var response = new ZdoStartupFromAppResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolZdoStartupFromAppStatus.NewNetworkState, response.Status);
    }

    [Fact]
    public void UtilLedControlResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.UtilLedControlRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x67, 0x0a]);

        var data = new byte[] { 0x01, 0x6d };
        var response = new UtilLedControlResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, response.Status);
    }

    [Fact]
    public void DeviceInfoResponseIeeeAddress()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x09;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZbGetDeviceInfoRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x66, 0x06]);

        var data = new byte[] { 0x01, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x60 };
        var response = new ZbGetDeviceInfoResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(DeviceInfoType.IeeeAddr, response.DeviceInfoType);
        Assert.Equal(0x0807060504030201u, response.Value);
    }

    [Fact]
    public void DeviceInfoResponsePanId()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x09;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZbGetDeviceInfoRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x66, 0x06]);

        var data = new byte[] { 0x06, 0x01, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6c };
        var response = new ZbGetDeviceInfoResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(DeviceInfoType.PanId, response.DeviceInfoType);
        Assert.Equal(0x0102u, response.Value);
    }

    [Fact]
    public void DeviceInfoResponseChannel()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x09;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZbGetDeviceInfoRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x66, 0x06]);

        var data = new byte[] { 0x05, 0xa1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xcd };
        var response = new ZbGetDeviceInfoResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(DeviceInfoType.Channel, response.DeviceInfoType);
        Assert.Equal(0xa1u, response.Value);
    }

    [Fact]
    public void ZdoMsgCbRegisterResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoMsgCbRegisterRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x3e]);

        var data = new byte[] { 0x01, 0x5b };
        var response = new ZdoMsgCbRegisterResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(response, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, response.Status);
    }

    [Fact]
    public void SysResetIndCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x06;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.SysResetIndClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x41, 0x80]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xc0 };
        var incPacket = new SysResetIndCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolDeviceResetReason.External, incPacket.Reason);
        Assert.Equal(0x02, incPacket.TransportRev);
        Assert.Equal(0x03, incPacket.ProductId);
        Assert.Equal(0x04, incPacket.MajorRel);
        Assert.Equal(0x05, incPacket.MinorRel);
        Assert.Equal(0x06, incPacket.HwRev);
    }

    [Fact]
    public void ZdoEndDeviceAnnceIndCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x0d;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoEndDeviceAnnceIndClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0xc1]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x88 };
        var incPacket = new ZdoEndDeviceAnnceIndCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.SrcAddr);
        Assert.Equal(0x0403u, incPacket.NwkAddr);
        Assert.Equal(0x05060708090a0b0cu, incPacket.IeeeAddr);
        Assert.True(incPacket.IsAlternativePanCoordinator);
        Assert.False(incPacket.IsZigBeeRouter);
        Assert.True(incPacket.IsMainPowered);
        Assert.True(incPacket.IsReceiverOnWhenIdle);
        Assert.False(incPacket.IsSecure);
    }

    [Fact]
    public void ZdoMsgCbIncomingCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x0a;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoMsgCbIncomingClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0xff]);

        var data = new byte[] { 0x01, 0x02, 0x01, 0x03, 0x04, 0x00, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0xb3 };
        var incPacket = new ZdoMsgCbIncomingCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.SrcAddr);
        Assert.True(incPacket.IsBroadcast);
        Assert.Equal(ZigBeeClusterId.PressureMeasurement, incPacket.ClusterId);
        Assert.Equal(0x07, incPacket.SeqNum);
        Assert.Equal(0x0908u, incPacket.MacDstAddr);
        var msgData = data[9..];

        for (var i = 0; i < incPacket.MsgData.Length; i++)
            Assert.Equal(msgData[i], incPacket.MsgData[i]);
    }

    [Fact]
    public void ZdoNodeDescResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoNodeDescRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x02]);

        var data = new byte[] { 0x01, 0x67 };
        var incPacket = new ZdoNodeDescResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
    }

    [Fact]
    public void ZdoNodeDescCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x12;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoNodeDescClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0x82]);

        var data = new byte[] { 0x01, 0x02, 0x01, 0x03, 0x04, 0x1a, 0x00, 0x00, 0x05, 0x06, 0x07, 0x08, 0x09, 0x26, 0x00, 0x0a, 0x0b, 0x00, 0xe8 };
        var incPacket = new ZdoNodeDescCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.SrcAddr);
        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
        Assert.Equal(0x0403u, incPacket.NwkAddrOfInterest);
        Assert.True(incPacket.IsUserDescriptorAvailable);
        Assert.True(incPacket.IsComplexDescriptorAvailable);
        Assert.Equal(ZigBeeDeviceType.EndDevice, incPacket.DeviceType);
        Assert.Equal(0x0605u, incPacket.ManufacturerCode);
        Assert.Equal(0x07u, incPacket.MaxBufferSize);
        Assert.Equal(0x0908u, incPacket.MaxTransferSize);
        Assert.True(incPacket.IsBackupDiscoveryCache);
        Assert.False(incPacket.IsPrimaryDiscoveryCache);
        Assert.False(incPacket.IsBackupBindTableCache);
        Assert.True(incPacket.IsPrimaryBindTableCache);
        Assert.True(incPacket.IsBackupTrustCenter);
        Assert.False(incPacket.IsPrimaryTrustCenter);
        Assert.Equal(0x0b0au, incPacket.MaxOutTransferSize);
    }

    [Fact]
    public void AfDataResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfDataRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x64, 0x01]);

        var data = new byte[] { 0x01, 0x65 };
        var incPacket = new AfDataResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
    }

    [Fact]
    public void ZdoNwkAddrCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x11;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoNwkAddrClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0x80]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x02, 0x20, 0x30, 0x40, 0x50, 0xda };
        var incPacket = new ZdoNwkAddrCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
        Assert.Equal(0x0203040506070809u, incPacket.IeeeAddr);
        Assert.Equal(0x0a0bu, incPacket.NwkAddr);
        Assert.Equal(0x0cu, incPacket.StartIndex);
        Assert.Equal(0x02u, incPacket.NumAssocDev);

        Assert.Collection(incPacket.AssocDevList,
            item => Assert.Equal(0x2030u, item),
            item => Assert.Equal(0x4050u, item));
    }

    [Fact]
    public void ZdoStateChangedIndCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoStateChangedIndClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0xc0]);

        var data = new byte[] { 0x02, 0x86 };
        var incPacket = new ZdoStateChangedIndCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolZdoState.NwkDisc, incPacket.State);
    }

    [Fact]
    public void ZdoNwkDiscoveryResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoNwkDiscoveryRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x26]);

        var data = new byte[] { 0xfc, 0xbe };
        var incPacket = new ZdoNwkDiscoveryResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.MacScanInProgress, incPacket.Status);
    }

    [Fact]
    public void SysPingResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x02;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.SysPingRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x61, 0x01]);

        var data = new byte[] { 0x79, 0x01, 0x1a };
        var incPacket = new SysPingResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.True(incPacket.IsSysCapable);
        Assert.False(incPacket.IsMacCapable);
        Assert.False(incPacket.IsNwkCapable);
        Assert.True(incPacket.IsAfCapable);
        Assert.True(incPacket.IsZdoCapable);
        Assert.True(incPacket.IsSapiCapable);
        Assert.True(incPacket.IsUtilCapable);
        Assert.False(incPacket.IsDebugCapable);
        Assert.True(incPacket.IsAppCapable);
        Assert.False(incPacket.IsZoadCapable);
    }

    [Fact]
    public void ZdoActiveEpResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoActiveEpRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x05]);

        var data = new byte[] { 0x01, 0x60 };
        var incPacket = new ZdoActiveEpResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
    }

    [Fact]
    public void ZdoActiveEpCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x08;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoActiveEpClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0x85]);

        var data = new byte[] { 0x00, 0x01, 0x01, 0x00, 0x02, 0x02, 0x03, 0x04, 0xcf };
        var incPacket = new ZdoActiveEpCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0001u, incPacket.SrcAddr);
        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
        Assert.Equal(0x0002u, incPacket.NwkAddr);
        Assert.Equal(0x02, incPacket.ActiveEpCount);

        Assert.Collection(incPacket.ActiveEps,
            item => Assert.Equal(0x03, item),
            item => Assert.Equal(0x04, item));
    }

    [Fact]
    public void AfRegisterResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfRegisterRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x64, 0x00]);

        var data = new byte[] { 0x01, 0x64 };
        var incPacket = new AfRegisterResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
    }

    [Fact]
    public void UtilGetDeviceInfoResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x12;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.UtilGetDeviceInfoRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x67, 0x00]);

        var data = new byte[] { 0x01, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x01, 0x03, 0x02, 0x01, 0x02, 0x03, 0x04, 0x7b };
        var incPacket = new UtilGetDeviceInfoResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
        Assert.Equal(0x0807060504030201u, incPacket.IeeeAddr);
        Assert.Equal(0x090au, incPacket.NwkAddr);
        Assert.True(incPacket.IsCoordinatorCapable);
        Assert.False(incPacket.IsRouterCapable);
        Assert.False(incPacket.IsEndDeviceCapable);
        Assert.Equal(UtilGetDeviceInfoDeviceState.JoiningPan, incPacket.State);
        Assert.Equal(0x02u, incPacket.NumAssocDevices);

        Assert.Collection(incPacket.AssocDevices,
            item => Assert.Equal(0x0201u, item),
            item => Assert.Equal(0x0403u, item));
    }

    [Fact]
    public void ZdoExtRouteDiscoveryResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoExtRouteDiscRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x45]);

        var data = new byte[] { 0x01, 0x20 };
        var incPacket = new ZdoExtRouteDiscoveryResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
    }

    [Fact]
    public void SysGetExtAddrResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x08;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.SysGetExtAddrRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x61, 0x04]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x65 };
        var incPacket = new SysGetExtAddrResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0807060504030201u, incPacket.ExtAddress);
    }

    [Fact]
    public void SysOsalNvLengthResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x02;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.SysOsalNvLengthRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x61, 0x13]);

        var data = new byte[] { 0x01, 0x02, 0x73 };
        var incPacket = new SysOsalNvLengthResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.Length);
    }

    [Fact]
    public void SysOsalNvReadResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x04;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.SysOsalNvReadRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x61, 0x08]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x69 };
        var incPacket = new SysOsalNvReadResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
        Assert.Equal(0x02u, incPacket.Len);
        Assert.Equal(data[2], incPacket.Value[0]);
        Assert.Equal(data[3], incPacket.Value[1]);
    }

    [Fact]
    public void ZdoExtFindGroupResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x03;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoExtFindGroupRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x4a]);

        var groupId = new byte[] { 0x01, 0x02, 0x03 };
        var data = new byte[] { groupId[0], groupId[1], groupId[2], 0x2c };
        var incPacket = new ZdoExtFindGroupResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Collection(incPacket.GroupId,
            item => Assert.Equal(groupId[0], item),
            item => Assert.Equal(groupId[1], item),
            item => Assert.Equal(groupId[2], item));
    }

    [Fact]
    public void ZdoSimpleDescResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoSimpleDescRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x04]);

        var data = new byte[] { 0x01, 0x61 };
        var incPacket = new ZdoSimpleDescResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
    }

    [Fact]
    public void ZdoSimpleDescCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x10;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoSimpleDescClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0x84]);

        var data = new byte[] { 0x01, 0x02, 0x01, 0x03, 0x04, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x00, 0x01, 0x02, 0x03, 0x01, 0x04, 0x05, 0xd5 };
        var incPacket = new ZdoSimpleDescCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.SrcAddr);
        Assert.Equal(ZToolPacketStatus.Fail, incPacket.Status);
        Assert.Equal(0x0403u, incPacket.NwkAddr);
        Assert.Equal(0x02, incPacket.Len);
        Assert.Equal(0x03, incPacket.Endpoint);
        Assert.Equal(0x0504u, incPacket.ProfileId);
        Assert.Equal(0x0706u, incPacket.DeviceId);
        Assert.Equal(0x00, incPacket.DeviceVersion);
        Assert.Equal(0x01, incPacket.NumInClusters);

        Assert.Equal(0x01, incPacket.NumOutClusters);

        var inCluster = Assert.Single(incPacket.InClusters);
        var outCluster = Assert.Single(incPacket.OutClusters);

        Assert.Equal(0x0302u, inCluster);
        Assert.Equal(0x0504u, outCluster);
    }

    [Fact]
    public void ZdoExtNwkInfoResponse()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x18;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoExtNwkInfoRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x50]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x35 };

        var incPacket = new ZdoExtNwkInfoResponse(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.ShortAddr);
        Assert.Equal(ZToolZdoState.NwkJoining, incPacket.DevState);
        Assert.Equal(0x0504u, incPacket.PanId);
        Assert.Equal(0x0706u, incPacket.ParentAddr);
        Assert.Equal(0x0f0e0d0c0b0a0908u, incPacket.ExtendedPanId);
        Assert.Equal(0x1716151413121110u, incPacket.ExtendedParentAddr);
        Assert.Equal(0x18, incPacket.Channel);
    }

    [Fact]
    public void AfIncomingMsgCallback()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x13;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfIncomingMsgClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x44, 0x81]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x00, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x02, 0x12, 0x13, 0xce };
        var incPacket = new AfIncomingMessageCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(0x0201u, incPacket.GroupId);
        Assert.Equal(ZigBeeClusterId.PressureMeasurement, incPacket.ClusterId);
        Assert.Equal(0x0605u, incPacket.SrcAddr);
        Assert.Equal(0x07, incPacket.SrcEndpoint);
        Assert.Equal(0x08, incPacket.DstEndpoint);
        Assert.True(incPacket.WasBroadcast);
        Assert.Equal(0x0a, incPacket.LinkQuality);
        Assert.False(incPacket.SecurityUse);
        Assert.Equal(0x0f0e0d0cu, incPacket.TimeStamp);
        Assert.Equal(0x10, incPacket.TransSeqNumber);
        Assert.Equal(0x02, incPacket.Len);
        Assert.Equal(2, incPacket.Message.Length);
        Assert.Equal(0x12, incPacket.Message[0]);
        Assert.Equal(0x13, incPacket.Message[1]);
    }

    [Fact]
    public void StartByteIncorrect()
    {
        // Arrange.
        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(0xee);

        var data = new byte[] { 0x02, 0x86 };
        var incPacket = new ZdoStateChangedIndCallback(_packetHeaderMock.Object, data);

        // Assert.
        Assert.False(incPacket.IsStartByteCorrect);

        _packetHeaderMock.VerifyAll();
    }

    [Fact]
    public void CheckSumIncorrect()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;

        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0xc0]);

        var data = new byte[] { 0x02, 0x84 };
        var incPacket = new ZdoStateChangedIndCallback(_packetHeaderMock.Object, data);

        // Assert.
        Assert.False(incPacket.IsCheckSumCorrect);

        _packetHeaderMock.VerifyAll();
    }

    [Fact]
    public void PacketFormatIncorrect()
    {
        // Arrange.
        const int ExpectedDataLenght = 0x05;

        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0xc0]);

        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xc0 };
        var incPacket = new SysResetIndCallback(_packetHeaderMock.Object, data);

        // Assert.
        Assert.False(incPacket.IsCorrectPacketFormat);

        _packetHeaderMock.VerifyAll();
    }

    [Theory]
    [InlineData(0x01, 0x64, ZToolPacketStatus.Fail)]
    [InlineData(0x00, 0x65, ZToolPacketStatus.Success)]
    public void ZdoIeeeAddrResponse(byte data, byte checkSum, ZToolPacketStatus expectedResult)
    {
        // Arrange.
        const int ExpectedDataLenght = 0x01;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoIeeeAddrRsp;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x65, 0x01]);

        var dataArray = new byte[] { data, checkSum };

        var incPacket = new ZdoIeeeAddrResponse(_packetHeaderMock.Object, dataArray);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(expectedResult, incPacket.Status);
    }

    [Fact]
    public void ZdoIeeeAddrCallback_NumAssocDevIsZero()
    {
        // Arrange.
        const ZToolPacketStatus ExpectedStatus = ZToolPacketStatus.Fail;
        const byte ExpectedStartIndex = 0;
        const byte ExpectedNumAssocDev = 0;
        const int ExpectedDataLenght = 0x0d;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoIeeeAddrClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0x81]);

        var data = new byte[]
        {
            (byte)ExpectedStatus,
            0x01,
            0x02,
            0x03,
            0x04,
            0x05,
            0x06,
            0x07,
            0x08,
            0x09,
            0x0a,
            ExpectedStartIndex,
            ExpectedNumAssocDev,
            0xc3
        };

        var incPacket = new ZdoIeeeAddrCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ExpectedStatus, incPacket.Status);
        Assert.Equal(0x0102030405060708ul, incPacket.IeeeAddr);
        Assert.Equal(0x0a09, incPacket.NwkAddr);
        Assert.Equal(ExpectedStartIndex, incPacket.StartIndex);
        Assert.Equal(ExpectedNumAssocDev, incPacket.NumAssocDev);
        Assert.Empty(incPacket.AssocDevList);
    }

    [Fact]
    public void ZdoIeeeAddrCallback_NumAssocDevIsNotZero()
    {
        // Arrange.
        const ZToolPacketStatus ExpectedStatus = ZToolPacketStatus.Fail;
        const byte ExpectedStartIndex = 0;
        const byte ExpectedNumAssocDev = 2;
        const int ExpectedDataLenght = 0x11;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.ZdoIeeeAddrClbk;

        _packetHeaderMock.SetupGet(m => m.StartByte).Returns(StartByte);
        _packetHeaderMock.SetupGet(m => m.DataLength).Returns(ExpectedDataLenght);
        _packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);
        _packetHeaderMock.Setup(m => m.ToByteArray()).Returns([StartByte, ExpectedDataLenght, 0x45, 0x81]);

        var data = new byte[]
        {
            (byte)ExpectedStatus,
            0x01,
            0x02,
            0x03,
            0x04,
            0x05,
            0x06,
            0x07,
            0x08,
            0x09,
            0x0a,
            ExpectedStartIndex,
            ExpectedNumAssocDev,
            0x01,
            0x02,
            0x03,
            0x04,
            0xd9
        };

        var incPacket = new ZdoIeeeAddrCallback(_packetHeaderMock.Object, data);

        // Assert.
        CheckGeneralData(incPacket, ExpectedDataLenght, ExpectedCmdType);

        _packetHeaderMock.VerifyAll();

        Assert.Equal(ExpectedStatus, incPacket.Status);
        Assert.Equal(0x0102030405060708ul, incPacket.IeeeAddr);
        Assert.Equal(0x0a09, incPacket.NwkAddr);
        Assert.Equal(ExpectedStartIndex, incPacket.StartIndex);
        Assert.Equal(ExpectedNumAssocDev, incPacket.NumAssocDev);
        Assert.Collection(incPacket.AssocDevList,
            item => Assert.Equal(0x0201, item),
            item => Assert.Equal(0x0403, item));
    }

    private static void CheckGeneralData(IncomingPacket response, byte expectedDataLen, ZToolCmdType expectedCmdType)
    {
        Assert.Equal(expectedDataLen, response.DataLength);
        Assert.Equal(expectedCmdType, response.CmdType);
        Assert.True(response.IsStartByteCorrect);
        Assert.True(response.IsCheckSumCorrect);
        Assert.True(response.IsCorrectPacketFormat);
        Assert.True(response.IsPacketCorrect, $"Packet {response.CmdType} is not correct.");
    }
}
