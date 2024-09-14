using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Devices;
public class NetworkCoordinatorTests
{
    private readonly Mock<IPacketReceiverTransmitterService> _packetReceiverTransmitterServiceMock = new();
    private readonly Mock<ILogger<NetworkCoordinator>> _loggerMock = new();

    [Fact]
    public void Initialize()
    {
        // Arrange.
        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        coordinator.Initialize();

        // Assert.
        _packetReceiverTransmitterServiceMock.Verify(m => m.Initialize(), Times.Once);
    }

    [Fact]
    public void GetCoordinatorInfo_RespondNotSuccess_ThrowsZigBeeNetworkException()
    {
        // Arrange.
        var responseMock = new Mock<IUtilGetDeviceInfoResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Fail);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(It.IsAny<UtilGetDeviceInfoRequest>(), ZToolCmdType.UtilGetDeviceInfoRsp))
            .Returns(responseMock.Object);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(coordinator.GetCoordinatorInfo);

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = coordinator.GetCoordinatorInfo();

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var exception = Assert.Throws<NetworkException>(coordinator.PingCoordinatorOrThrow);

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        coordinator.PingCoordinatorOrThrow();

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
    }

    [Fact]
    public void ResetCoordinatorDevice()
    {
        // Arrange.
        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        coordinator.ResetCoordinator();

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = coordinator.SetCoordinatorLedMode(LedId, LedOn);

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = coordinator.SetCoordinatorLedMode(LedId, LedOn);

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = coordinator.StartupNetwork(Delay);

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = coordinator.StartupNetwork(Delay);

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = coordinator.PermitNetworkJoin(TransactionId, isPermitted);

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
    [InlineData(true)]
    [InlineData(false)]
    public void PermitJoin_ReturnsTrue(bool isPermitted)
    {
        // Arrange.
        const int TransactionId = 123;

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

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        var result = coordinator.PermitNetworkJoin(TransactionId, isPermitted);

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

    #region NetworkEndpoints
    [Fact]
    public void GetSupportedEndpoints()
    {
        // Arrange.
        var endpointIds = new byte[] { 1, 2, 3, 4, 5, 6, 8, 10, 11, 12, 13, 47, 110, 242 };

        var coordinator = new NetworkCoordinator(null!, null!);

        // Act.
        var result = coordinator.GetSupportedEndpoints();

        // Assert.
        Assert.Collection(result,
            item => Assert.Equal(endpointIds[0], item.EndpointId),
            item => Assert.Equal(endpointIds[1], item.EndpointId),
            item => Assert.Equal(endpointIds[2], item.EndpointId),
            item => Assert.Equal(endpointIds[3], item.EndpointId),
            item => Assert.Equal(endpointIds[4], item.EndpointId),
            item => Assert.Equal(endpointIds[5], item.EndpointId),
            item => Assert.Equal(endpointIds[6], item.EndpointId),
            item => Assert.Equal(endpointIds[7], item.EndpointId),
            item => Assert.Equal(endpointIds[8], item.EndpointId),
            item => Assert.Equal(endpointIds[9], item.EndpointId),
            item => Assert.Equal(endpointIds[10], item.EndpointId),
            item => Assert.Equal(endpointIds[11], item.EndpointId),
            item => Assert.Equal(endpointIds[12], item.EndpointId),
            item => Assert.Equal(endpointIds[13], item.EndpointId));
    }

    [Fact]
    public void GetActiveEndpointIds_ThrowsNetworkException()
    {
        // Arrange.
        var callbackMock = new Mock<IZdoActiveEpCallback>();

        callbackMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.ZSecNoKey);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoActiveEpCallback>(It.Is<ZdoActiveEpRequest>(r => r.DestAddr == 0 && r.NwkAddrOfInterest == 0), ZToolCmdType.ZdoActiveEpClbk))
            .Returns(callbackMock.Object);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(coordinator.GetActiveEndpointIds);

        _packetReceiverTransmitterServiceMock.VerifyAll();
        callbackMock.VerifyAll();

        Assert.Equal("Active endpoint request failed.", result.Message);
    }

    [Fact]
    public void GetActiveEndpointIds_Success()
    {
        // Arrange.
        var expectedResult = new byte[] { 0x03, 0x04 };

        var callbackMock = new Mock<IZdoActiveEpCallback>();

        callbackMock.SetupGet(m => m.ActiveEps).Returns(expectedResult);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoActiveEpCallback>(It.Is<ZdoActiveEpRequest>(r => r.DestAddr == 0 && r.NwkAddrOfInterest == 0), ZToolCmdType.ZdoActiveEpClbk))
            .Returns(callbackMock.Object);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act.
        var result = coordinator.GetActiveEndpointIds();

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        callbackMock.VerifyAll();

        Assert.Collection(result,
            item => Assert.Equal(expectedResult[0], item),
            item => Assert.Equal(expectedResult[1], item));
    }

    [Fact]
    public void ValidateRegisteredEndpoints_SendAndWaitForResponseThrowsException_ThrowsNetworkException()
    {
        // Arrange.
        const byte EndpointId1 = 1;
        const byte EndpointId2 = 2;

        var internalException = new Exception();
        var callbackMock = new Mock<IZdoSimpleDescCallback>();

        callbackMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoSimpleDescCallback>(It.Is<ZdoSimpleDescRequest>(r => r.DestAddr == 0
            && r.NwkAddrOfInterest == 0
            && r.Endpoint == EndpointId1), ZToolCmdType.ZdoSimpleDescClbk))
            .Returns(callbackMock.Object);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoSimpleDescCallback>(It.Is<ZdoSimpleDescRequest>(r => r.DestAddr == 0
            && r.NwkAddrOfInterest == 0
            && r.Endpoint == EndpointId2), ZToolCmdType.ZdoSimpleDescClbk))
            .Throws(internalException);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(() => coordinator.ValidateRegisteredEndpoints([EndpointId1, EndpointId2]));

        _packetReceiverTransmitterServiceMock.VerifyAll();
        callbackMock.VerifyAll();

        Assert.Equal($"Endpoint validation failed {EndpointId2}.", result.Message);
        Assert.Equal(internalException, result.InnerException);
    }

    [Fact]
    public void ValidateRegisteredEndpoints_RequestFailed_ThrowsNetworkException()
    {
        // Arrange.
        const byte EndpointId1 = 1;
        const byte EndpointId2 = 2;

        var internalException = new Exception();
        var callbackMock1 = new Mock<IZdoSimpleDescCallback>();
        var callbackMock2 = new Mock<IZdoSimpleDescCallback>();

        callbackMock1.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);
        callbackMock2.SetupGet(m => m.Status).Returns(ZToolPacketStatus.ZApsNotSupported);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoSimpleDescCallback>(It.Is<ZdoSimpleDescRequest>(r => r.DestAddr == 0
            && r.NwkAddrOfInterest == 0
            && r.Endpoint == EndpointId1), ZToolCmdType.ZdoSimpleDescClbk))
            .Returns(callbackMock1.Object);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoSimpleDescCallback>(It.Is<ZdoSimpleDescRequest>(r => r.DestAddr == 0
            && r.NwkAddrOfInterest == 0
            && r.Endpoint == EndpointId2), ZToolCmdType.ZdoSimpleDescClbk))
            .Returns(callbackMock2.Object);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(() => coordinator.ValidateRegisteredEndpoints([EndpointId1, EndpointId2]));

        _packetReceiverTransmitterServiceMock.VerifyAll();
        callbackMock1.VerifyAll();
        callbackMock2.VerifyAll();

        Assert.Equal($"Endpoint validation failed {EndpointId2}.", result.Message);
        Assert.Null(result.InnerException);
    }

    [Fact]
    public void ValidateRegisteredEndpoints_Success()
    {
        // Arrange.
        const byte EndpointId1 = 1;
        const byte EndpointId2 = 2;

        var incomingMessageMock1 = new Mock<IZdoSimpleDescCallback>();
        var incomingMessageMock2 = new Mock<IZdoSimpleDescCallback>();

        incomingMessageMock1.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);
        incomingMessageMock2.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoSimpleDescCallback>(It.Is<ZdoSimpleDescRequest>(r => r.DestAddr == 0
            && r.NwkAddrOfInterest == 0
            && r.Endpoint == EndpointId1), ZToolCmdType.ZdoSimpleDescClbk))
            .Returns(incomingMessageMock1.Object);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IZdoSimpleDescCallback>(It.Is<ZdoSimpleDescRequest>(r => r.DestAddr == 0
            && r.NwkAddrOfInterest == 0
            && r.Endpoint == EndpointId2), ZToolCmdType.ZdoSimpleDescClbk))
            .Returns(incomingMessageMock2.Object);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. 
        coordinator.ValidateRegisteredEndpoints([EndpointId1, EndpointId2]);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        incomingMessageMock1.VerifyAll();
        incomingMessageMock2.VerifyAll();
    }

    [Fact]
    public void RegisterEndpoints_SendAndWaitForResponseThrowsException_ThrowsNetworkException()
    {
        // Arrange.
        var internalException = new Exception();

        var appInClusterList1 = new ushort[]
        {
            1,
            2
        };

        var appOutClusterList1 = new ushort[]
        {
            10,
            20
        };

        var appInClusterList2 = new ushort[]
        {
            3,
            4
        };

        var appOutClusterList2 = new ushort[]
        {
            30,
            40
        };

        var endpoints = new NetworkEndpoint[]
        {
            new(11, 12, 13, 14, AfRegisterLatency.SlowBeacons, 15, appInClusterList1, 16, appOutClusterList1),
            new(21, 22, 23, 24, AfRegisterLatency.FastBeacons, 25, appInClusterList2, 26, appOutClusterList2)
        };

        var incomingMessageMock1 = new Mock<IAfRegisterResponse>();

        incomingMessageMock1.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfRegisterResponse>(It.Is<AfRegisterRequest>(r => r.Endpoint == endpoints[0].EndpointId
            && r.AppProfId == endpoints[0].AppProfId
            && r.AppDeviceId == endpoints[0].AppDeviceId
            && r.AppDevVersion == endpoints[0].AppDevVersion
            && r.LatencyReq == endpoints[0].Latency
            && r.AppNumInClusters == endpoints[0].AppNumInClusters
            && r.AppInClusterList == endpoints[0].AppInClusterList
            && r.AppNumOutClusters == endpoints[0].AppNumOutClusters
            && r.AppOutClusterList == endpoints[0].AppOutClusterList), ZToolCmdType.AfRegisterRsp))
            .Returns(incomingMessageMock1.Object);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfRegisterResponse>(It.Is<AfRegisterRequest>(r => r.Endpoint == endpoints[1].EndpointId
            && r.AppProfId == endpoints[1].AppProfId
            && r.AppDeviceId == endpoints[1].AppDeviceId
            && r.AppDevVersion == endpoints[1].AppDevVersion
            && r.LatencyReq == endpoints[1].Latency
            && r.AppNumInClusters == endpoints[1].AppNumInClusters
            && r.AppInClusterList == endpoints[1].AppInClusterList
            && r.AppNumOutClusters == endpoints[1].AppNumOutClusters
            && r.AppOutClusterList == endpoints[1].AppOutClusterList), ZToolCmdType.AfRegisterRsp))
            .Throws(internalException);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(() => coordinator.RegisterEndpoints(endpoints));

        _packetReceiverTransmitterServiceMock.VerifyAll();
        incomingMessageMock1.VerifyAll();
        _loggerMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal($"Cannot register endpoint {endpoints[1].EndpointId}", result.Message);
        Assert.Equal(internalException, result.InnerException);
    }

    [Fact]
    public void RegisterEndpoints_RequestFailed_ThrowsNetworkException()
    {
        // Arrange.
        var appInClusterList1 = new ushort[]
        {
            1,
            2
        };

        var appOutClusterList1 = new ushort[]
        {
            10,
            20
        };

        var appInClusterList2 = new ushort[]
        {
            3,
            4
        };

        var appOutClusterList2 = new ushort[]
        {
            30,
            40
        };

        var endpoints = new NetworkEndpoint[]
        {
            new(11, 12, 13, 14, AfRegisterLatency.SlowBeacons, 15, appInClusterList1, 16, appOutClusterList1),
            new(21, 22, 23, 24, AfRegisterLatency.FastBeacons, 25, appInClusterList2, 26, appOutClusterList2)
        };

        var incomingMessageMock1 = new Mock<IAfRegisterResponse>();
        var incomingMessageMock2 = new Mock<IAfRegisterResponse>();

        incomingMessageMock1.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);
        incomingMessageMock2.SetupGet(m => m.Status).Returns(ZToolPacketStatus.ZMacInvalidGts);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfRegisterResponse>(It.Is<AfRegisterRequest>(r => r.Endpoint == endpoints[0].EndpointId
            && r.AppProfId == endpoints[0].AppProfId
            && r.AppDeviceId == endpoints[0].AppDeviceId
            && r.AppDevVersion == endpoints[0].AppDevVersion
            && r.LatencyReq == endpoints[0].Latency
            && r.AppNumInClusters == endpoints[0].AppNumInClusters
            && r.AppInClusterList == endpoints[0].AppInClusterList
            && r.AppNumOutClusters == endpoints[0].AppNumOutClusters
            && r.AppOutClusterList == endpoints[0].AppOutClusterList), ZToolCmdType.AfRegisterRsp))
            .Returns(incomingMessageMock1.Object);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfRegisterResponse>(It.Is<AfRegisterRequest>(r => r.Endpoint == endpoints[1].EndpointId
            && r.AppProfId == endpoints[1].AppProfId
            && r.AppDeviceId == endpoints[1].AppDeviceId
            && r.AppDevVersion == endpoints[1].AppDevVersion
            && r.LatencyReq == endpoints[1].Latency
            && r.AppNumInClusters == endpoints[1].AppNumInClusters
            && r.AppInClusterList == endpoints[1].AppInClusterList
            && r.AppNumOutClusters == endpoints[1].AppNumOutClusters
            && r.AppOutClusterList == endpoints[1].AppOutClusterList), ZToolCmdType.AfRegisterRsp))
            .Returns(incomingMessageMock2.Object);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(() => coordinator.RegisterEndpoints(endpoints));

        _packetReceiverTransmitterServiceMock.VerifyAll();
        incomingMessageMock1.VerifyAll();
        incomingMessageMock2.VerifyAll();
        _loggerMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal($"Cannot register endpoint {endpoints[1].EndpointId}", result.Message);
        Assert.Null(result.InnerException);
    }

    [Fact]
    public void RegisterEndpoints_Success()
    {
        // Arrange.
        var appInClusterList1 = new ushort[]
        {
            1,
            2
        };

        var appOutClusterList1 = new ushort[]
        {
            10,
            20
        };

        var appInClusterList2 = new ushort[]
        {
            3,
            4
        };

        var appOutClusterList2 = new ushort[]
        {
            30,
            40
        };

        var endpoints = new NetworkEndpoint[]
        {
            new(11, 12, 13, 14, AfRegisterLatency.SlowBeacons, 15, appInClusterList1, 16, appOutClusterList1),
            new(21, 22, 23, 24, AfRegisterLatency.FastBeacons, 25, appInClusterList2, 26, appOutClusterList2)
        };

        var incomingMessageMock1 = new Mock<IAfRegisterResponse>();
        var incomingMessageMock2 = new Mock<IAfRegisterResponse>();

        incomingMessageMock1.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);
        incomingMessageMock2.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Success);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfRegisterResponse>(It.Is<AfRegisterRequest>(r => r.Endpoint == endpoints[0].EndpointId
            && r.AppProfId == endpoints[0].AppProfId
            && r.AppDeviceId == endpoints[0].AppDeviceId
            && r.AppDevVersion == endpoints[0].AppDevVersion
            && r.LatencyReq == endpoints[0].Latency
            && r.AppNumInClusters == endpoints[0].AppNumInClusters
            && r.AppInClusterList == endpoints[0].AppInClusterList
            && r.AppNumOutClusters == endpoints[0].AppNumOutClusters
            && r.AppOutClusterList == endpoints[0].AppOutClusterList), ZToolCmdType.AfRegisterRsp))
            .Returns(incomingMessageMock1.Object);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IAfRegisterResponse>(It.Is<AfRegisterRequest>(r => r.Endpoint == endpoints[1].EndpointId
            && r.AppProfId == endpoints[1].AppProfId
            && r.AppDeviceId == endpoints[1].AppDeviceId
            && r.AppDevVersion == endpoints[1].AppDevVersion
            && r.LatencyReq == endpoints[1].Latency
            && r.AppNumInClusters == endpoints[1].AppNumInClusters
            && r.AppInClusterList == endpoints[1].AppInClusterList
            && r.AppNumOutClusters == endpoints[1].AppNumOutClusters
            && r.AppOutClusterList == endpoints[1].AppOutClusterList), ZToolCmdType.AfRegisterRsp))
            .Returns(incomingMessageMock2.Object);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Information)).Returns(true);

        var coordinator = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            _loggerMock.Object);

        // Act.
        coordinator.RegisterEndpoints(endpoints);

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        incomingMessageMock1.VerifyAll();
        incomingMessageMock2.VerifyAll();
        _loggerMock.VerifyAll();

        _loggerMock.Verify(x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(endpoints.Length));
    }
    #endregion NetworkEndpoints
}
