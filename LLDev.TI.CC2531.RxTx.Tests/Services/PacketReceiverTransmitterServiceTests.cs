using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Options;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class PacketReceiverTransmitterServiceTests
{
    private const int Timeout = 200;

    private readonly Mock<IPacketHandler> _serialPortMessageHandlerMock = new();

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

        using var service = new PacketReceiverTransmitterService(_serialPortMessageHandlerMock.Object, null!, _options);

        // Act.
        service.Send(outgoingPacketMock.Object);

        // Assert.
        _serialPortMessageHandlerMock.Verify(m => m.Send(outgoingPacketMock.Object), Times.Once);
    }

    [Fact]
    public void SendAndWaitForResponse() => Assert.Fail("Implement me");
}
