using Castle.Core.Logging;
using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Devices;
public class NetworkCoordinatorTests
{
    private readonly Mock<IPacketReceiverTransmitterService> _packetReceiverTransmitterServiceMock = new();
    private readonly Mock<ITransactionService> _transactionServiceMock = new();
    private readonly Mock<ILogger<NetworkCoordinator>> _loggerMock = new();

    [Fact]
    public void GetCoordinatorInfo_RespondNotSuccess_ThrowsZigBeeNetworkException()
    {
        // Arrange.
        var responseMock = new Mock<IUtilGetDeviceInfoResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Fail);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(It.IsAny<UtilGetDeviceInfoRequest>(), ZToolCmdType.UtilGetDeviceInfoRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!,
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
            null!,
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
            null!,
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
            null!,
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
        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        service.ResetCoordinator();

        // Assert.
        _loggerMock.VerifyAll();

        _packetReceiverTransmitterServiceMock.Verify(m => m.SendAndWaitForResponse<ISysResetIndCallback>(It.Is<SysResetRequest>(r => r.ResetType == ZToolSysResetType.SerialBootloader),
            ZToolCmdType.SysResetIndClbk), Times.Once);

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
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

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        responseMock.SetupGet(m => m.Status).Returns(responseStatus);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilLedControlResponse>(It.Is<UtilLedControlRequest>(r => r.LedId == LedId && r.LedOn == LedOn), ZToolCmdType.UtilLedControlRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        var result = service.SetCoordinatorLedMode(LedId, LedOn);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
        responseMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.False(result);
    }

    [Fact]
    public void SetCoordinatorLedMode_ReturnsTrue()
    {
        // Arrange.
        const int LedId = 123;
        const bool LedOn = true;

        var responseMock = new Mock<IUtilLedControlResponse>();

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilLedControlResponse>(It.Is<UtilLedControlRequest>(r => r.LedId == LedId && r.LedOn == LedOn), ZToolCmdType.UtilLedControlRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        var result = service.SetCoordinatorLedMode(LedId, LedOn);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
        responseMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.True(result);
    }

    [Fact]
    public void StartupNetwork_NetworkNotStarted()
    {
        // Arrange.
        const ushort Delay = 1234;

        var responseMock = new Mock<IZdoStartupFromAppResponse>();

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        responseMock.SetupGet(m => m.Status).Returns(ZToolZdoStartupFromAppStatus.NotStarted);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoStartupFromAppResponse>(It.Is<ZdoStartupFromAppRequest>(r => r.StartDelay == Delay), ZToolCmdType.ZdoStartupFromAppRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        var result = service.StartupNetwork(Delay);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
        responseMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

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

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        responseMock.SetupGet(m => m.Status).Returns(expectedStatus);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoStartupFromAppResponse>(It.Is<ZdoStartupFromAppRequest>(r => r.StartDelay == Delay), ZToolCmdType.ZdoStartupFromAppRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!,
            _loggerMock.Object);

        // Act.
        var result = service.StartupNetwork(Delay);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
        responseMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.True(result);
    }

    [Theory]
    [InlineData(ZToolPacketStatus.Fail, true)]
    [InlineData(ZToolPacketStatus.InvalidMemorySize, false)]
    [InlineData(ZToolPacketStatus.NvOperationFailed, true)]
    [InlineData(ZToolPacketStatus.ZdpInsufficientSpace, false)]
    internal void PermitNetworkJoin_RespondNotSuccess_ReturnsFalse(ZToolPacketStatus statusCode, bool isPermitted)
    {
        // Arrange.
        const int TransactionId = 123;

        _transactionServiceMock.Setup(m => m.GetNextTransactionId()).Returns(TransactionId);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var responseMock = new Mock<IAfDataResponse>();

        responseMock.SetupGet(m => m.Status).Returns(statusCode);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfDataResponse>(It.Is<AfDataRequest>(r => r.NwkDstAddr == 0 &&
            r.DstEndpoint == 0 &&
            r.SrcEndpoint == 0 &&
            r.ClusterId == ZigBeeClusterId.PermitJoin &&
            r.TransactionId == TransactionId &&
            r.Options == 0x30 &&
            r.Radius == 31 &&
            r.RequestDataLen == 3 &&
            r.RequestData[0] == TransactionId &&
            r.RequestData[1] == (isPermitted ? 255 : 0) &&
            r.RequestData[2] == 1), ZToolCmdType.AfDataRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = service.PermitNetworkJoin(isPermitted);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        _transactionServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
        responseMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.False(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PermitJoin_ReturnsTrue(bool isPermitted)
    {
        // Arrange.
        const int TransactionId = 123;

        _transactionServiceMock.Setup(m => m.GetNextTransactionId()).Returns(TransactionId);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var responseMock = new Mock<IAfDataResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfDataResponse>(It.Is<AfDataRequest>(r => r.NwkDstAddr == 0 &&
            r.DstEndpoint == 0 &&
            r.SrcEndpoint == 0 &&
            r.ClusterId == ZigBeeClusterId.PermitJoin &&
            r.TransactionId == TransactionId &&
            r.Options == 0x30 &&
            r.Radius == 31 &&
            r.RequestDataLen == 3 &&
            r.RequestData[0] == TransactionId &&
            r.RequestData[1] == (isPermitted ? 255 : 0) &&
            r.RequestData[2] == 1), ZToolCmdType.AfDataRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _transactionServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = service.PermitNetworkJoin(isPermitted);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        _transactionServiceMock.VerifyAll();
        _loggerMock.VerifyAll();
        responseMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.True(result);
    }
}
