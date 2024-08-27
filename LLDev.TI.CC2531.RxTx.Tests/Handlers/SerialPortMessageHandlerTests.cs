using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Handlers;
public class SerialPortMessageHandlerTests
{
    private readonly Mock<ISerialPortDataHandler> _serialPortDataHandlerMock = new();
    private readonly Mock<IPacketHeaderFactory> _packetHeaderFactoryMock = new();
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
    public void SerialPortDataReceived_CreatePacketHeaderThrowsPacketHeaderException()
    {
        // Arrange.
        var dataToReadCount = 0;

        var headerArray = new byte[] { 1, 2, 3 };

        var exception = new PacketHeaderException();

        _serialPortDataHandlerMock.SetupGet(m => m.IsDataToRead).Returns(() =>
        {
            var result = dataToReadCount == 0;
            dataToReadCount++;

            return result;
        });

        _serialPortDataHandlerMock.Setup(m => m.Read(Constants.HeaderLength)).Returns(headerArray);

        _packetHeaderFactoryMock.Setup(m => m.CreatePacketHeader(headerArray)).Throws(exception);

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Error)).Returns(true);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            null!,
            _packetHeaderFactoryMock.Object,
            _loggerMock.Object);

        // Act.
        _serialPortDataHandlerMock.Raise(m => m.DataReceived += null);

        // Assert.
        _serialPortDataHandlerMock.VerifyAll();
        _packetHeaderFactoryMock.VerifyAll();
        _loggerMock.VerifyAll();

        _serialPortDataHandlerMock.VerifyGet(m => m.IsDataToRead, Times.Exactly(2));

        _loggerMock.Verify(x => x.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            exception,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        _serialPortDataHandlerMock.Verify(m => m.FlushIncomingData(), Times.Once);
    }

    [Fact]
    public void Test123() => Assert.Fail("Implement me");
}
