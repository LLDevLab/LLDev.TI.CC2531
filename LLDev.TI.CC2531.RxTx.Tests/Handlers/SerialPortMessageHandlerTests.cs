using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Handlers;
public class SerialPortMessageHandlerTests
{
    private readonly Mock<ISerialPortDataHandler> _serialPortDataHandlerMock = new();
    private readonly Mock<IPacketFactory> _packetFactoryMock = new();
    private readonly Mock<ILogger<SerialPortMessageHandler>> _loggerMock = new();

    [Fact]
    public void ConstructAndDispose()
    {
        // Act.
        using (var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object, null!, null!, null!))
        {
        }

        // Assert.
        _serialPortDataHandlerMock.Verify(m => m.Open());
        _serialPortDataHandlerMock.VerifyAdd(m => m.DataReceived += It.IsAny<SerialPortDataReceivedEventHandler>());
        _serialPortDataHandlerMock.VerifyRemove(m => m.DataReceived -= It.IsAny<SerialPortDataReceivedEventHandler>());
        _serialPortDataHandlerMock.Verify(m => m.Dispose());
    }

    [Fact]
    public void Send_MessageAlreadySending_ThrowsInvalidOperationException()
    {
        // Arrange.
        const ZToolCmdType ResultType = ZToolCmdType.SysResetIndClbk;

        var outgoingPacketMock = new Mock<IOutgoingPacket>();

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object, null!, null!, null!);

        handler.Send(outgoingPacketMock.Object, null!, ResultType);

        // Act. / Assert.
        var result = Assert.Throws<InvalidOperationException>(() => handler.Send(outgoingPacketMock.Object, null!, ResultType));

        _serialPortDataHandlerMock.VerifyAll();

        Assert.Equal($"Request for result type {ResultType} already sending.", result.Message);
    }

    [Fact]
    public void Send()
    {
        // Arrange.
        const ZToolCmdType ResultType = ZToolCmdType.SysResetIndClbk;

        var bytesToSend = new byte[] { 0x01, 0x02, 0x03 };

        var outgoingPacketMock = new Mock<IOutgoingPacket>();
        outgoingPacketMock.Setup(m => m.ToByteArray()).Returns(bytesToSend);

        _serialPortDataHandlerMock.Setup(m => m.Write(bytesToSend));

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object, null!, null!, null!);

        // Act.
        handler.Send(outgoingPacketMock.Object, null!, ResultType);

        // Assert.
        outgoingPacketMock.VerifyAll();
        _serialPortDataHandlerMock.VerifyAll();
    }

    [Fact]
    public void Test123() => Assert.Fail("Implement me");
}
