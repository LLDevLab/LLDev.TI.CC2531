using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Options;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class PacketReceiverTransmitterServiceTests
{
    private const int Timeout = 200;

    private readonly Mock<IPacketHandler> _serialPortMessageHandlerMock = new();
    private readonly Mock<ICmdTypeValidationService> _cmdTypeValidationServiceMock = new();
    private readonly Mock<IAwaitedPacketCacheService> _awaitedPacketCacheServiceMock = new();

    private readonly IOptions<SerialPortMessageServiceConfig> _options = Options.Create(new SerialPortMessageServiceConfig
    {
        MessageWaitTimeoutMs = Timeout,
    });

    [Fact]
    public void CondtructAndDispose()
    {
        // Arrange. / Act.
        using (var service = new PacketReceiverTransmitterService(_serialPortMessageHandlerMock.Object,
            null!,
            null!,
            _options))
        {
        }

        // Assert.
        _serialPortMessageHandlerMock.VerifyAdd(m => m.MessageReceivedAsync += It.IsAny<MessageReceivedHandler>());
        _serialPortMessageHandlerMock.VerifyRemove(m => m.MessageReceivedAsync -= It.IsAny<MessageReceivedHandler>());
    }

    [Fact]
    public void Send()
    {
        // Arrange.
        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        using var service = new PacketReceiverTransmitterService(_serialPortMessageHandlerMock.Object, null!, null!, _options);

        // Act.
        service.Send(outgoingPacketMock.Object);

        // Assert.
        _serialPortMessageHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);
    }

    [Fact]
    public void SendAndWaitForResponse_CommandTypeIsNotResponseOrCallback_ThrowsArgumentException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.AfRegisterReq;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        _cmdTypeValidationServiceMock.Setup(m => m.IsResponseOrCallback(CmdType)).Returns(false);

        using var service = new PacketReceiverTransmitterService(_serialPortMessageHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            null!,
            _options);

        // Act. / Assert.
        var exception = Assert.Throws<ArgumentException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _serialPortMessageHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();

        _serialPortMessageHandlerMock.Verify(m => m.Send(It.IsAny<IOutgoingPacket>()), Times.Never);

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

        using var service = new PacketReceiverTransmitterService(_serialPortMessageHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object,
            _options);

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _serialPortMessageHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();

        _serialPortMessageHandlerMock.Verify(m => m.Send(It.IsAny<IOutgoingPacket>()), Times.Never);
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

        using var service = new PacketReceiverTransmitterService(_serialPortMessageHandlerMock.Object,
            _cmdTypeValidationServiceMock.Object,
            _awaitedPacketCacheServiceMock.Object,
            _options);

        // Act. / Assert.
        var exception = Assert.Throws<TimeoutException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _serialPortMessageHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();
        _awaitedPacketCacheServiceMock.VerifyAll();

        _serialPortMessageHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);

        _awaitedPacketCacheServiceMock.Verify(m => m.Add(CmdType), Times.Once);
        _awaitedPacketCacheServiceMock.VerifyNoOtherCalls();

        Assert.Equal($"Cannot receive response within specified duretion {Timeout} ms", exception.Message);
    }

    [Fact]
    public void SendAndWaitForResponse() => Assert.Fail("Implement me");
}
