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
    public void GetDeviceInfo_DeviceNotRespond_ThrowsNetworkException()
    {
        // Arrange.
        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(It.IsAny<UtilGetDeviceInfoRequest>(), ZToolCmdType.UtilGetDeviceInfoRsp))
            .Returns((IUtilGetDeviceInfoResponse)null!);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(service.GetDeviceInfo);

        _packetReceiverTransmitterServiceMock.VerifyAll();

        Assert.Equal("Cannot receive network coordinator info", result.Message);
    }

    [Fact]
    public void GetNetworkCoordinatorInfo_RespondNotSuccess_ThrowsZigBeeNetworkException()
    {
        // Arrange.
        var responseMock = new Mock<IUtilGetDeviceInfoResponse>();

        responseMock.SetupGet(m => m.Status).Returns(ZToolPacketStatus.Fail);

        _packetReceiverTransmitterServiceMock.Setup(m => m.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(It.IsAny<UtilGetDeviceInfoRequest>(), ZToolCmdType.UtilGetDeviceInfoRsp))
            .Returns(responseMock.Object);

        var service = new NetworkCoordinator(_packetReceiverTransmitterServiceMock.Object,
            null!);

        // Act. / Assert.
        var result = Assert.Throws<NetworkException>(service.GetDeviceInfo);

        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.Equal("Cannot receive network coordinator info", result.Message);
    }

    [Fact]
    public void GetDeviceInfo()
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
        var result = service.GetDeviceInfo();

        // Assert.
        _packetReceiverTransmitterServiceMock.VerifyAll();
        responseMock.VerifyAll();

        Assert.Equal(IeeeAddr, result.IeeeAddr);
        Assert.Equal(NwkAddr, result.NwkAddr);
    }

    [Fact]
    public void Fail() => Assert.Fail("Implement me");
}
