using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Tests.Handlers;
public class SerialPortMessageHandlerTests
{
    private readonly Mock<ISerialPortDataHandler> _serialPortDataHandlerMock = new();
    private readonly Mock<IPacketHeaderFactory> _packetHeaderFactoryMock = new();
    private readonly Mock<IPacketFactory> _packetFactoryMock = new();
    private readonly Mock<IMessageCallbackMethodsCacheService> _messageCallbackMethodsCacheServiceMock = new();
    private readonly Mock<ILogger<SerialPortMessageHandler>> _loggerMock = new();

    [Fact]
    public void ConstructAndDispose()
    {
        // Act.
        using (var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object, null!, null!, null!, null!))
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

        _messageCallbackMethodsCacheServiceMock.Setup(m => m.ContainsKey(ResultType)).Returns(true);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            null!,
            null!,
            _messageCallbackMethodsCacheServiceMock.Object,
            null!);

        handler.Send(outgoingPacketMock.Object, null!, ResultType);

        // Act. / Assert.
        var result = Assert.Throws<InvalidOperationException>(() => handler.Send(outgoingPacketMock.Object, null!, ResultType));

        _messageCallbackMethodsCacheServiceMock.VerifyAll();
        _serialPortDataHandlerMock.VerifyAll();

        _messageCallbackMethodsCacheServiceMock.VerifyNoOtherCalls();

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

        _messageCallbackMethodsCacheServiceMock.Setup(m => m.ContainsKey(ResultType)).Returns(false);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            null!,
            null!,
            _messageCallbackMethodsCacheServiceMock.Object,
            null!);

        // Act.
        handler.Send(outgoingPacketMock.Object, null!, ResultType);

        // Assert.
        _messageCallbackMethodsCacheServiceMock.VerifyAll();
        outgoingPacketMock.VerifyAll();

        _messageCallbackMethodsCacheServiceMock.Verify(m => m.Add(ResultType, It.IsAny<Action<IIncomingPacket?>>()), Times.Once);

        _serialPortDataHandlerMock.Verify(m => m.Write(bytesToSend), Times.Once);
    }

    [Fact]
    public void SerialPortDataReceived_CreatePacketHeaderThrowsPacketHeaderException()
    {
        // Arrange.
        var dataToReadCount = 0;
        var messageReceivedEventInvokeCount = 0;

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
            null!,
            _loggerMock.Object);

        handler.MessageReceivedAsync += incomintPacket => messageReceivedEventInvokeCount++;

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

        Assert.Equal(0, messageReceivedEventInvokeCount);
    }

    [Fact]
    public void SerialPortDataReceived_PacketIsNull()
    {
        // Arrange.
        const int PacketHeaderDataLen = 10;
        const int CheckSum = 100;

        var dataToReadCount = 0;
        var messageReceivedEventInvokeCount = 0;

        var headerArray = new byte[] { 1, 2, 3 };
        var packetArray = new byte[] { 5, 6, 7 };

        var packetHeaderMock = new Mock<IPacketHeader>();

        packetHeaderMock.SetupGet(m => m.DataLength).Returns(PacketHeaderDataLen);

        _serialPortDataHandlerMock.SetupGet(m => m.IsDataToRead).Returns(() =>
        {
            var result = dataToReadCount == 0;
            dataToReadCount++;

            return result;
        });

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Warning)).Returns(true);

        _serialPortDataHandlerMock.Setup(m => m.Read(Constants.HeaderLength)).Returns(headerArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(PacketHeaderDataLen)).Returns(packetArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(1)).Returns([CheckSum]);

        _packetHeaderFactoryMock.Setup(m => m.CreatePacketHeader(headerArray)).Returns(packetHeaderMock.Object);

        _packetFactoryMock.Setup(m => m.CreateIncomingPacket(packetHeaderMock.Object, It.Is<byte[]>(a => a.Length == packetArray.Length + 1 &&
            a[0] == packetArray[0] &&
            a[1] == packetArray[1] &&
            a[2] == packetArray[2] &&
            a[3] == CheckSum
        ))).Returns((IIncomingPacket?)null);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            _packetFactoryMock.Object,
            _packetHeaderFactoryMock.Object,
            null!,
            _loggerMock.Object);

        handler.MessageReceivedAsync += incomintPacket => messageReceivedEventInvokeCount++;

        // Act.
        _serialPortDataHandlerMock.Raise(m => m.DataReceived += null);

        // Assert.
        _serialPortDataHandlerMock.VerifyAll();
        _packetHeaderFactoryMock.VerifyAll();
        packetHeaderMock.VerifyAll();
        _packetFactoryMock.VerifyAll();
        _loggerMock.VerifyAll();

        _serialPortDataHandlerMock.VerifyGet(m => m.IsDataToRead, Times.Exactly(2));

        _serialPortDataHandlerMock.Verify(m => m.FlushIncomingData(), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal(0, messageReceivedEventInvokeCount);
    }

    [Fact]
    public void SerialPortDataReceived_PacketIsIncorrect()
    {
        // Arrange.
        const int PacketHeaderDataLen = 10;
        const int CheckSum = 100;

        var dataToReadCount = 0;
        var messageReceivedEventInvokeCount = 0;

        var headerArray = new byte[] { 1, 2, 3 };
        var packetArray = new byte[] { 5, 6, 7 };

        var packetHeaderMock = new Mock<IPacketHeader>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        packetHeaderMock.SetupGet(m => m.DataLength).Returns(PacketHeaderDataLen);

        incomingPacketMock.SetupGet(m => m.IsPacketCorrect).Returns(false);

        _serialPortDataHandlerMock.SetupGet(m => m.IsDataToRead).Returns(() =>
        {
            var result = dataToReadCount == 0;
            dataToReadCount++;

            return result;
        });

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Warning)).Returns(true);

        _serialPortDataHandlerMock.Setup(m => m.Read(Constants.HeaderLength)).Returns(headerArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(PacketHeaderDataLen)).Returns(packetArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(1)).Returns([CheckSum]);

        _packetHeaderFactoryMock.Setup(m => m.CreatePacketHeader(headerArray)).Returns(packetHeaderMock.Object);

        _packetFactoryMock.Setup(m => m.CreateIncomingPacket(packetHeaderMock.Object, It.Is<byte[]>(a => a.Length == packetArray.Length + 1 &&
            a[0] == packetArray[0] &&
            a[1] == packetArray[1] &&
            a[2] == packetArray[2] &&
            a[3] == CheckSum
        ))).Returns(incomingPacketMock.Object);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            _packetFactoryMock.Object,
            _packetHeaderFactoryMock.Object,
            null!,
            _loggerMock.Object);

        handler.MessageReceivedAsync += incomintPacket => messageReceivedEventInvokeCount++;

        // Act.
        _serialPortDataHandlerMock.Raise(m => m.DataReceived += null);

        // Assert.
        _serialPortDataHandlerMock.VerifyAll();
        _packetHeaderFactoryMock.VerifyAll();
        packetHeaderMock.VerifyAll();
        incomingPacketMock.VerifyAll();
        _packetFactoryMock.VerifyAll();
        _loggerMock.VerifyAll();

        _serialPortDataHandlerMock.VerifyGet(m => m.IsDataToRead, Times.Exactly(2));

        _serialPortDataHandlerMock.Verify(m => m.FlushIncomingData(), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal(0, messageReceivedEventInvokeCount);
    }

    [Fact]
    public void SerialPortDataReceived_CallbackExistsInCache_CallbackActionIsNull()
    {
        // Arrange.
        const int PacketHeaderDataLen = 10;
        const int CheckSum = 100;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfDataReq;

        var dataToReadCount = 0;
        var messageReceivedEventInvokeCount = 0;

        var headerArray = new byte[] { 1, 2, 3 };
        var packetArray = new byte[] { 5, 6, 7 };

        var packetHeaderMock = new Mock<IPacketHeader>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        packetHeaderMock.SetupGet(m => m.DataLength).Returns(PacketHeaderDataLen);
        packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);

        incomingPacketMock.SetupGet(m => m.IsPacketCorrect).Returns(true);

        _serialPortDataHandlerMock.SetupGet(m => m.IsDataToRead).Returns(() =>
        {
            var result = dataToReadCount == 0;
            dataToReadCount++;

            return result;
        });

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Trace)).Returns(true);

        _serialPortDataHandlerMock.Setup(m => m.Read(Constants.HeaderLength)).Returns(headerArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(PacketHeaderDataLen)).Returns(packetArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(1)).Returns([CheckSum]);

        _packetHeaderFactoryMock.Setup(m => m.CreatePacketHeader(headerArray)).Returns(packetHeaderMock.Object);

        _packetFactoryMock.Setup(m => m.CreateIncomingPacket(packetHeaderMock.Object, It.Is<byte[]>(a => a.Length == packetArray.Length + 1 &&
            a[0] == packetArray[0] &&
            a[1] == packetArray[1] &&
            a[2] == packetArray[2] &&
            a[3] == CheckSum
        ))).Returns(incomingPacketMock.Object);

        _messageCallbackMethodsCacheServiceMock.Setup(m => m.ContainsKey(ExpectedCmdType)).Returns(true);
        _messageCallbackMethodsCacheServiceMock.Setup(m => m.GetAndRemove(ExpectedCmdType)).Returns((Action<IIncomingPacket?>?)null);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            _packetFactoryMock.Object,
            _packetHeaderFactoryMock.Object,
            _messageCallbackMethodsCacheServiceMock.Object,
            _loggerMock.Object);

        handler.MessageReceivedAsync += incomintPacket => messageReceivedEventInvokeCount++;

        // Act.
        _serialPortDataHandlerMock.Raise(m => m.DataReceived += null);

        // Assert.
        _serialPortDataHandlerMock.VerifyAll();
        _packetHeaderFactoryMock.VerifyAll();
        packetHeaderMock.VerifyAll();
        incomingPacketMock.VerifyAll();
        _packetFactoryMock.VerifyAll();
        _messageCallbackMethodsCacheServiceMock.VerifyAll();
        _loggerMock.VerifyAll();

        _serialPortDataHandlerMock.VerifyGet(m => m.IsDataToRead, Times.Exactly(2));

        _serialPortDataHandlerMock.Verify(m => m.FlushIncomingData(), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Trace,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal(0, messageReceivedEventInvokeCount);
    }

    [Fact]
    public void SerialPortDataReceived_CallbackExistsInCache_CallbackActionIsNotNull()
    {
        // Arrange.
        const int PacketHeaderDataLen = 10;
        const int CheckSum = 100;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfDataReq;

        var dataToReadCount = 0;
        var messageReceivedEventInvokeCount = 0;
        var callbackActionInvocationCount = 0;

        var headerArray = new byte[] { 1, 2, 3 };
        var packetArray = new byte[] { 5, 6, 7 };

        var packetHeaderMock = new Mock<IPacketHeader>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        packetHeaderMock.SetupGet(m => m.DataLength).Returns(PacketHeaderDataLen);
        packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);

        incomingPacketMock.SetupGet(m => m.IsPacketCorrect).Returns(true);

        _serialPortDataHandlerMock.SetupGet(m => m.IsDataToRead).Returns(() =>
        {
            var result = dataToReadCount == 0;
            dataToReadCount++;

            return result;
        });

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Trace)).Returns(true);

        _serialPortDataHandlerMock.Setup(m => m.Read(Constants.HeaderLength)).Returns(headerArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(PacketHeaderDataLen)).Returns(packetArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(1)).Returns([CheckSum]);

        _packetHeaderFactoryMock.Setup(m => m.CreatePacketHeader(headerArray)).Returns(packetHeaderMock.Object);

        _packetFactoryMock.Setup(m => m.CreateIncomingPacket(packetHeaderMock.Object, It.Is<byte[]>(a => a.Length == packetArray.Length + 1 &&
            a[0] == packetArray[0] &&
            a[1] == packetArray[1] &&
            a[2] == packetArray[2] &&
            a[3] == CheckSum
        ))).Returns(incomingPacketMock.Object);

        _messageCallbackMethodsCacheServiceMock.Setup(m => m.ContainsKey(ExpectedCmdType)).Returns(true);
        _messageCallbackMethodsCacheServiceMock.Setup(m => m.GetAndRemove(ExpectedCmdType)).Returns(incommingPacket => callbackActionInvocationCount++);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            _packetFactoryMock.Object,
            _packetHeaderFactoryMock.Object,
            _messageCallbackMethodsCacheServiceMock.Object,
            _loggerMock.Object);

        handler.MessageReceivedAsync += incomintPacket => messageReceivedEventInvokeCount++;

        // Act.
        _serialPortDataHandlerMock.Raise(m => m.DataReceived += null);

        // Assert.
        _serialPortDataHandlerMock.VerifyAll();
        _packetHeaderFactoryMock.VerifyAll();
        packetHeaderMock.VerifyAll();
        incomingPacketMock.VerifyAll();
        _packetFactoryMock.VerifyAll();
        _messageCallbackMethodsCacheServiceMock.VerifyAll();
        _loggerMock.VerifyAll();

        _serialPortDataHandlerMock.VerifyGet(m => m.IsDataToRead, Times.Exactly(2));

        _serialPortDataHandlerMock.Verify(m => m.FlushIncomingData(), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Trace,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal(0, messageReceivedEventInvokeCount);
        Assert.Equal(1, callbackActionInvocationCount);
    }

    [Fact]
    public void SerialPortDataReceived_CallbackNotExistInCache()
    {
        // Arrange.
        const int PacketHeaderDataLen = 10;
        const int CheckSum = 100;
        const ZToolCmdType ExpectedCmdType = ZToolCmdType.AfDataReq;

        var dataToReadCount = 0;
        var messageReceivedEventInvokeCount = 0;

        var headerArray = new byte[] { 1, 2, 3 };
        var packetArray = new byte[] { 5, 6, 7 };

        var packetHeaderMock = new Mock<IPacketHeader>();
        var incomingPacketMock = new Mock<IIncomingPacket>();

        packetHeaderMock.SetupGet(m => m.DataLength).Returns(PacketHeaderDataLen);
        packetHeaderMock.SetupGet(m => m.CmdType).Returns(ExpectedCmdType);

        incomingPacketMock.SetupGet(m => m.IsPacketCorrect).Returns(true);

        _serialPortDataHandlerMock.SetupGet(m => m.IsDataToRead).Returns(() =>
        {
            var result = dataToReadCount == 0;
            dataToReadCount++;

            return result;
        });

        _loggerMock.Setup(m => m.IsEnabled(LogLevel.Trace)).Returns(true);

        _serialPortDataHandlerMock.Setup(m => m.Read(Constants.HeaderLength)).Returns(headerArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(PacketHeaderDataLen)).Returns(packetArray);
        _serialPortDataHandlerMock.Setup(m => m.Read(1)).Returns([CheckSum]);

        _packetHeaderFactoryMock.Setup(m => m.CreatePacketHeader(headerArray)).Returns(packetHeaderMock.Object);

        _packetFactoryMock.Setup(m => m.CreateIncomingPacket(packetHeaderMock.Object, It.Is<byte[]>(a => a.Length == packetArray.Length + 1 &&
            a[0] == packetArray[0] &&
            a[1] == packetArray[1] &&
            a[2] == packetArray[2] &&
            a[3] == CheckSum
        ))).Returns(incomingPacketMock.Object);

        _messageCallbackMethodsCacheServiceMock.Setup(m => m.ContainsKey(ExpectedCmdType)).Returns(false);

        var handler = new SerialPortMessageHandler(_serialPortDataHandlerMock.Object,
            _packetFactoryMock.Object,
            _packetHeaderFactoryMock.Object,
            _messageCallbackMethodsCacheServiceMock.Object,
            _loggerMock.Object);

        handler.MessageReceivedAsync += incomintPacket => messageReceivedEventInvokeCount++;

        // Act.
        _serialPortDataHandlerMock.Raise(m => m.DataReceived += null);

        // Assert.
        _serialPortDataHandlerMock.VerifyAll();
        _packetHeaderFactoryMock.VerifyAll();
        packetHeaderMock.VerifyAll();
        incomingPacketMock.VerifyAll();
        _packetFactoryMock.VerifyAll();
        _messageCallbackMethodsCacheServiceMock.VerifyAll();
        _loggerMock.VerifyAll();

        _serialPortDataHandlerMock.VerifyGet(m => m.IsDataToRead, Times.Exactly(2));

        _serialPortDataHandlerMock.Verify(m => m.FlushIncomingData(), Times.Never);

        _loggerMock.Verify(x => x.Log(LogLevel.Trace,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        Assert.Equal(1, messageReceivedEventInvokeCount);
    }
}
