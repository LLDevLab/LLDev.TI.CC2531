using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class PacketReceiverTransmitterServiceTests
{
    private const int Timeout = 100;

    private readonly Mock<IPacketHandler> _packetHandlerMock = new();
    private readonly Mock<ICmdTypeValidationService> _cmdTypeValidationServiceMock = new();
    private readonly Mock<IAwaitedPacketCacheService> _awaitedPacketCacheServiceMock = new();

    [Fact]
    public void CondtructAndDispose()
    {
        // Arrange. / Act.
        using (var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            null!,
            null!))
        {
        }

        // Assert.
        _packetHandlerMock.VerifyAdd(m => m.PacketReceived += It.IsAny<PacketReceivedHandler>());
        _packetHandlerMock.VerifyRemove(m => m.PacketReceived -= It.IsAny<PacketReceivedHandler>());
    }

    [Fact]
    public void Initialize()
    {
        // Arrange.
        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            null!,
            null!);

        // Act.
        service.Initialize();

        // Assert.
        _packetHandlerMock.Verify(m => m.Initialize(), Times.Once);
    }

    [Fact]
    public void Send()
    {
        // Arrange.
        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object, null!, null!);

        // Act.
        service.Send(outgoingPacketMock.Object);

        // Assert.
        _packetHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);
    }

    [Fact]
    public void SendAndWaitForResponse_CommandTypeIsNotResponseOrCallback_ThrowsArgumentException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.AfRegisterReq;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(CmdType)).Returns(false);

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            null!);

        // Act. / Assert.
        var exception = Assert.Throws<ArgumentException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(It.IsAny<IOutgoingPacket>()), Times.Never);

        Assert.Equal("Awaited response type is not response or callback (Parameter 'responseType')", exception.Message);
    }

    [Fact]
    public void SendAndWaitForResponse_MessageAlreadyAwaiting_ThrowsPacketException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.AfIncomingMsgClbk;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(CmdType)).Returns(true);

        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(CmdType)).Returns(true);

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object);

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(It.IsAny<IOutgoingPacket>()), Times.Never);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal($"Already awaiting packet {CmdType}", exception.Message);
    }

    [Fact]
    public void SendAndWaitForResponse_ResponceDoNotReceiver_ThrowsTimeoutException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.AfIncomingMsgClbk;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(CmdType)).Returns(true);

        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(CmdType)).Returns(false);

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object);

        // Act. / Assert.
        var exception = Assert.Throws<TimeoutException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);

        _awaitedPacketCacheServiceMock.Verify(m => m.Add(CmdType), Times.Once);
        _awaitedPacketCacheServiceMock.Verify(m => m.Remove(It.IsAny<ZToolCmdType>()), Times.Never);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal($"Cannot receive response within specified duretion {Timeout} ms", exception.Message);
    }

    [Fact]
    public void SendAndWaitForResponse_PacketNotAwaited()
    {
        // Arrange.
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfIncomingMsgClbk;
        const ZToolCmdType IncomingCmdType = ZToolCmdType.ZdoMsgCbIncomingClbk;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        var notAwaitedPacketReceivedCount = 0;

        incomingPacketMock.SetupGet(m => m.CmdType).Returns(IncomingCmdType);

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(ExpectedCmdType)).Returns(true);

        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(ExpectedCmdType)).Returns(false);
        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(IncomingCmdType)).Returns(false);

        _packetHandlerMock.Setup(m => m.Send(outgoingPacketMock.Object)).Callback((IOutgoingPacket _) =>
            _packetHandlerMock.Raise(m => m.PacketReceived += null, incomingPacketMock.Object));

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object);

        service.PacketReceived += OnPacketReceived;

        // Act. / Assert.
        Assert.Throws<TimeoutException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, ExpectedCmdType));

        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();
        incomingPacketMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);

        _awaitedPacketCacheServiceMock.Verify(m => m.Add(ExpectedCmdType), Times.Once);
        _awaitedPacketCacheServiceMock.Verify(m => m.Remove(It.IsAny<ZToolCmdType>()), Times.Never);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal(1, notAwaitedPacketReceivedCount);

        void OnPacketReceived(IIncomingPacket packet) => notAwaitedPacketReceivedCount++;
    }

    [Fact]
    public void SendAndWaitForResponse_DifferentAwaitedPacketFilteredOutFromAnotherAwaitedPacketHandler()
    {
        // Arrange.
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfIncomingMsgClbk;
        const ZToolCmdType IncomingCmdType = ZToolCmdType.ZdoMsgCbIncomingClbk;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        var notAwaitedPacketReceivedCount = 0;

        incomingPacketMock.SetupGet(m => m.CmdType).Returns(IncomingCmdType);

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(ExpectedCmdType)).Returns(true);

        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(ExpectedCmdType)).Returns(false);
        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(IncomingCmdType)).Returns(true);

        _packetHandlerMock.Setup(m => m.Send(outgoingPacketMock.Object)).Callback((IOutgoingPacket _) =>
            _packetHandlerMock.Raise(m => m.PacketReceived += null, incomingPacketMock.Object));

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object);

        service.PacketReceived += OnPacketReceived;

        // Act. / Assert.
        Assert.Throws<TimeoutException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, ExpectedCmdType));

        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();
        incomingPacketMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);

        _awaitedPacketCacheServiceMock.Verify(m => m.Add(ExpectedCmdType), Times.Once);
        _awaitedPacketCacheServiceMock.Verify(m => m.Remove(It.IsAny<ZToolCmdType>()), Times.Never);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal(0, notAwaitedPacketReceivedCount);

        void OnPacketReceived(IIncomingPacket packet) => notAwaitedPacketReceivedCount++;
    }

    [Fact]
    public void SendAndWaitForResponse_CannotCastIncomingPacketToAwaitedType_PacketException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.AfIncomingMsgClbk;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        var containsCounter = 0;
        var notAwaitedPacketReceivedCount = 0;

        incomingPacketMock.SetupGet(m => m.CmdType).Returns(CmdType);

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(CmdType)).Returns(true);

        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(CmdType)).Returns(() =>
        {
            var result = containsCounter != 0;

            containsCounter++;

            return result;
        });

        _packetHandlerMock.Setup(m => m.Send(outgoingPacketMock.Object)).Callback((IOutgoingPacket _) =>
            _packetHandlerMock.Raise(m => m.PacketReceived += null, incomingPacketMock.Object));

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object);

        service.PacketReceived += OnPacketReceived;

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();
        incomingPacketMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);

        _awaitedPacketCacheServiceMock.Verify(m => m.Add(CmdType), Times.Once);
        _awaitedPacketCacheServiceMock.Verify(m => m.Remove(CmdType), Times.Once);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal($"Cannot cast packet to {typeof(ZbWriteConfigResponse)}", exception.Message);
        Assert.Equal(0, notAwaitedPacketReceivedCount);

        void OnPacketReceived(IIncomingPacket packet) => notAwaitedPacketReceivedCount++;
    }

    [Fact]
    public void SendAndWaitForResponse()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.SysPingRsp;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        var containsCounter = 0;
        var notAwaitedPacketReceivedCount = 0;

        var packetHeaderMock = new Mock<IPacketHeader>();

        var incomingPacket = new SysPingResponse(packetHeaderMock.Object, [1, 2, 3]);

        packetHeaderMock.SetupGet(m => m.CmdType).Returns(CmdType);

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(CmdType)).Returns(true);

        _awaitedPacketCacheServiceMock.Setup(m => m.Contains(CmdType)).Returns(() =>
        {
            var result = containsCounter != 0;

            containsCounter++;

            return result;
        });

        _packetHandlerMock.Setup(m => m.Send(outgoingPacketMock.Object)).Callback((IOutgoingPacket _) =>
            _packetHandlerMock.Raise(m => m.PacketReceived += null, incomingPacket));

        using var service = new PacketReceiverTransmitterService(_packetHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object);

        service.PacketReceived += OnPacketReceived;

        // Act.
        var result = service.SendAndWaitForResponse<SysPingResponse>(outgoingPacketMock.Object, CmdType);

        // Assert.
        _packetHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();
        packetHeaderMock.VerifyAll();

        _packetHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);

        _awaitedPacketCacheServiceMock.Verify(m => m.Add(CmdType), Times.Once);
        _awaitedPacketCacheServiceMock.Verify(m => m.Remove(CmdType), Times.Once);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal(0, notAwaitedPacketReceivedCount);
        Assert.Equal(incomingPacket, result);

        void OnPacketReceived(IIncomingPacket packet) => notAwaitedPacketReceivedCount++;
    }
}
