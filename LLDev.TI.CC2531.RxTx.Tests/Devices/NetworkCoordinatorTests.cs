﻿using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Devices;
public class NetworkCoordinatorTests
{
    private readonly Mock<IPacketReceiverTransmitterService> _packetReceiverTransmitterServiceMock = new();
    private readonly Mock<ITransactionService> _transactionServiceMock = new();

    [Fact]
    public void GetCoordinatorInfo_RespondNotSuccess_ThrowsZigBeeNetworkException()
    {
        // Arrange.
        var responseMock = new Mock<IUtilGetDeviceInfoResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Fail);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(It.IsAny<UtilGetDeviceInfoRequest>(), ZToolCmdType.UtilGetDeviceInfoRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(service.GetCoordinatorInfo);

        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.Equal("Cannot receive network coordinator info", result.Message);
    }

    [Fact]
    public void GetCoordinatorInfo()
    {
        // Arrange.
        const ulong IeeeAddr = 12345;
        const ushort NwkAddr = 321;

        var responseMock = new Mock<IUtilGetDeviceInfoResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);
        responseMock.SetupGet(m => m.IeeeAddr).Returns(IeeeAddr);
        responseMock.SetupGet(m => m.NwkAddr).Returns(NwkAddr);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(It.IsAny<UtilGetDeviceInfoRequest>(), ZToolCmdType.UtilGetDeviceInfoRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = service.GetCoordinatorInfo();

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.Equal(IeeeAddr, result.IeeeAddr);
        Assert.Equal(NwkAddr, result.NwkAddr);
    }

    [Fact]
    public void PingCoordinatorOrThrow_ThrowsNetworkException()
    {
        // Arrange.
        var innerException = new Exception();

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<ISysPingResponse>(It.IsAny<SysPingRequest>(), ZToolCmdType.SysPingRsp))
            .Throws(innerException);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var exception = Assert.Throws<NetworkException>(service.PingCoordinatorOrThrow);

        _packetReceiverTransmitterServiceMock.VerifyAll();

        Assert.Equal("Cannot ping network coordinator", exception.Message);
    }

    [Fact]
    public void PingCoordinatorOrThrow()
    {
        // Arrange.
        var responseMock = new Mock<ISysPingResponse>();

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<ISysPingResponse>(It.IsAny<SysPingRequest>(), ZToolCmdType.SysPingRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        service.PingCoordinatorOrThrow();

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
    }

    [Fact]
    public void ResetCoordinatorDevice()
    {
        // Arrange.
        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        service.ResetCoordinator();

        // Assert.
        _packetReceiverTransmitterServiceMock.Verify(m => m.SendAndWaitForResponse<ISysResetIndCallback>(It.Is<SysResetRequest>(r => r.ResetType == ZToolSysResetType.SerialBootloader),
            ZToolCmdType.SysResetIndClbk), Times.Once);
    }

    [Theory]
    [InlineData(ZToolPacketStatus.Fail)]
    [InlineData(ZToolPacketStatus.InvalidEventId)]
    [InlineData(ZToolPacketStatus.InvalidTask)]
    [InlineData(ZToolPacketStatus.ZdpTableFull)]
    internal void SetCoordinatorLedMode_ReturnsFalse(ZToolPacketStatus responseStatus)
    {
        // Arrange.
        const int LedId = 123;
        const bool LedOn = true;

        var responseMock = new Mock<IUtilLedControlResponse>();

        responseMock.SetupGet(m => m.Status).Returns(responseStatus);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilLedControlResponse>(It.Is<UtilLedControlRequest>(r => r.LedId == LedId && r.LedOn == LedOn), ZToolCmdType.UtilLedControlRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = service.SetCoordinatorLedMode(LedId, LedOn);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.False(result);
    }

    [Fact]
    public void SetCoordinatorLedMode_ReturnsTrue()
    {
        // Arrange.
        const int LedId = 123;
        const bool LedOn = true;

        var responseMock = new Mock<IUtilLedControlResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilLedControlResponse>(It.Is<UtilLedControlRequest>(r => r.LedId == LedId && r.LedOn == LedOn), ZToolCmdType.UtilLedControlRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = service.SetCoordinatorLedMode(LedId, LedOn);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.True(result);
    }

    [Fact]
    public void StartupNetwork_NetworkNotStarted()
    {
        // Arrange.
        const ushort Delay = 1234;

        var responseMock = new Mock<IZdoStartupFromAppResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolZdoStartupFromAppStatus.NotStarted);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoStartupFromAppResponse>(It.Is<ZdoStartupFromAppRequest>(r => r.StartDelay == Delay), ZToolCmdType.ZdoStartupFromAppRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = service.StartupNetwork(Delay);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.False(result);
    }

    [Theory]
    [InlineData(ZToolZdoStartupFromAppStatus.NewNetworkState)]
    [InlineData(ZToolZdoStartupFromAppStatus.RestoredNwkState)]
    internal void StartupNetwork_Success(ZToolZdoStartupFromAppStatus expectedStatus)
    {
        // Arrange.
        const ushort Delay = 1234;

        var responseMock = new Mock<IZdoStartupFromAppResponse>();

        responseMock.SetupGet(m => m.Status).Returns(expectedStatus);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoStartupFromAppResponse>(It.Is<ZdoStartupFromAppRequest>(r => r.StartDelay == Delay), ZToolCmdType.ZdoStartupFromAppRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = service.StartupNetwork(Delay);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.True(result);
    }

    [Fact]
    public void Fail() => Assert.Fail("Implement me");
}
