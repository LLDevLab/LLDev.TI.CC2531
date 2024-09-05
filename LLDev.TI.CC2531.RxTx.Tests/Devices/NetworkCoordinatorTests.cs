using LLDev.TI.CC2531.RxTx.Devices;
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
    public void Fail() => Assert.Fail("Implement me");
}
