using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Enums;
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
            null!,
            _cmdTypeValidationServiceMock.Object,
            _options);

        // Act. / Assert.
        var exception = Assert.Throws<ArgumentException>(() => service.SendAndWaitForResponse<ZbWriteConfigResponse>(outgoingPacketMock.Object, CmdType));

        _serialPortMessageHandlerMock.VerifyAll();
        _cmdTypeValidationServiceMock.VerifyAll();

        Assert.Equal("Awaited response type is not response or callback", exception.Message);
    }

    [Fact]
    public void SendAndWaitForResponse() => Assert.Fail("Implement me");
}
