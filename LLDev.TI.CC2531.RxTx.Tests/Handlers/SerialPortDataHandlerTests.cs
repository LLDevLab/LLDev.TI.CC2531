using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using System.IO.Ports;
using System.Reflection;

namespace LLDev.TI.CC2531.RxTx.Tests.Handlers;
public class SerialPortDataHandlerTests
{
    private readonly Mock<ISerialPortHandler> _serialPortHandlerMock = new();

    [Fact]
    public void IsDataToRead_PortIsClosed()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(false);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        var result = handler.IsDataToRead;

        // Assert.
        _serialPortHandlerMock.VerifyAll();

        _serialPortHandlerMock.VerifyGet(m => m.BytesToRead, Times.Never);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(10, true)]
    public void IsDataToRead_PortIsOpen(int bytesToRead, bool excpetedResult)
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);
        _serialPortHandlerMock.SetupGet(m => m.BytesToRead).Returns(bytesToRead);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        var result = handler.IsDataToRead;

        // Assert.
        _serialPortHandlerMock.VerifyAll();

        Assert.Equal(excpetedResult, result);
    }

    [Fact]
    public void SerialPortHandler_Constructor()
    {
        // Act.
        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Assert.
        _serialPortHandlerMock.VerifyAdd(m => m.SerialDataReceived += It.IsAny<SerialDataReceivedEventHandler>());
    }

    [Fact]
    public void Write_PortIsClosed_ThrowsSerialPosrException()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(false);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act. / Assert.
        var exception = Assert.Throws<SerialPortException>(() => handler.Write([1, 2, 3]));

        _serialPortHandlerMock.VerifyAll();

        Assert.Equal("Serial port is closed.", exception.Message);
    }

    [Fact]
    public void Write()
    {
        // Arrange.
        var data = new byte[3] { 1, 2, 3 };

        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.Write(data);

        // Assert.
        _serialPortHandlerMock.VerifyAll();
        _serialPortHandlerMock.Verify(m => m.Write(data, 0, data.Length), Times.Once);
    }

    [Fact]
    public void Read_PortIsClosed_ThrowsSerialPortException()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(false);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act. / Assert.
        var exception = Assert.Throws<SerialPortException>(() => handler.Read(10));

        _serialPortHandlerMock.VerifyAll();

        Assert.Equal("Serial port is closed.", exception.Message);
    }

    [Fact]
    public void Read_ZeroBytes()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.Read(0);

        // Assert.
        _serialPortHandlerMock.VerifyAll();
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Read_NoMoreDataToRead()
    {
        // Arrange.
        const int FirstTimeRead = 2;

        var data = new byte[] { 1, 2 };

        var bytesToReadExecutionCount = 0;

        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);
        _serialPortHandlerMock.SetupGet(m => m.BytesToRead).Returns(() =>
        {
            var result = bytesToReadExecutionCount == 0 ? FirstTimeRead : 0;

            bytesToReadExecutionCount++;

            return result;
        });

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        _serialPortHandlerMock.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns<byte[], int, int>((arr, offset, toRead) =>
        {
            for (var i = offset; i < data.Length; i++)
                arr[i] = data[i];

            return data.Length;
        });

        // Act.
        var result = handler.Read(data.Length + 2);

        // Assert.
        _serialPortHandlerMock.VerifyAll();
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), 0, data.Length), Times.Once);
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), FirstTimeRead, data.Length - FirstTimeRead), Times.Once);

        Assert.Collection(result,
            item => Assert.Equal(data[0], item),
            item => Assert.Equal(data[1], item),
            item => Assert.Equal(data[2], item));
    }

    [Fact]
    public void Read()
    {
        // Arrange.
        const int FirstTimeRead = 2;

        var data = new byte[] { 1, 2, 3 };

        var executionCount = 0;

        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        _serialPortHandlerMock.Setup(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns<byte[], int, int>((arr, offset, toRead) =>
        {
            var readData = executionCount == 0 ? FirstTimeRead : data.Length - FirstTimeRead;

            for (var i = offset; i < offset + readData; i++)
                arr[i] = data[i];

            executionCount++;

            return readData;
        });

        // Act.
        var result = handler.Read(data.Length);

        // Assert.
        _serialPortHandlerMock.VerifyAll();
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), 0, data.Length), Times.Once);
        _serialPortHandlerMock.Verify(m => m.Read(It.IsAny<byte[]>(), FirstTimeRead, data.Length - FirstTimeRead), Times.Once);

        Assert.Collection(result,
            item => Assert.Equal(data[0], item),
            item => Assert.Equal(data[1], item),
            item => Assert.Equal(data[2], item));
    }

    [Fact]
    public void Open_AlreadyOpened()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.Open();

        // Assert.
        _serialPortHandlerMock.VerifyAll();

        _serialPortHandlerMock.Verify(m => m.Open(), Times.Never);
        _serialPortHandlerMock.Verify(m => m.DiscardInBuffer(), Times.Never);
        _serialPortHandlerMock.Verify(m => m.DiscardOutBuffer(), Times.Never);
    }

    [Fact]
    public void Open_ThrowsSerialPortException()
    {
        // Arrange.
        var innerException = new FileNotFoundException();

        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(false);
        _serialPortHandlerMock.Setup(m => m.Open()).Throws(innerException);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act. Assert.
        var exception = Assert.Throws<SerialPortException>(handler.Open);

        _serialPortHandlerMock.VerifyAll();

        _serialPortHandlerMock.Verify(m => m.DiscardInBuffer(), Times.Never);
        _serialPortHandlerMock.Verify(m => m.DiscardOutBuffer(), Times.Never);

        Assert.Equal("Serial port not found", exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void Open()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(false);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.Open();

        // Assert.
        _serialPortHandlerMock.VerifyAll();

        _serialPortHandlerMock.Verify(m => m.Open(), Times.Once);
        _serialPortHandlerMock.Verify(m => m.DiscardInBuffer(), Times.Once);
        _serialPortHandlerMock.Verify(m => m.DiscardOutBuffer(), Times.Once);
    }

    [Fact]
    public void Close_AlreadyClosed()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(false);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.Close();

        // Assert.
        _serialPortHandlerMock.VerifyAll();
        _serialPortHandlerMock.Verify(m => m.Close(), Times.Never);
    }

    [Fact]
    public void Close()
    {
        // Arrange.
        _serialPortHandlerMock.SetupGet(m => m.IsOpen).Returns(true);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.Close();

        // Assert.
        _serialPortHandlerMock.VerifyAll();

        _serialPortHandlerMock.Verify(m => m.Close(), Times.Once);
    }

    [Theory]
    [InlineData(SerialData.Chars, true)]
    [InlineData(SerialData.Eof, false)]
    public void DataReceivedEvent(SerialData dataType, bool expectedInvocation)
    {
        // Arrange.
        var isEventRaised = false;

        var constructor = typeof(SerialDataReceivedEventArgs).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            null, [typeof(SerialData)], null);

        var eventArgs =
            (SerialDataReceivedEventArgs?)constructor?.Invoke([dataType]);

        var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);
        handler.DataReceived += onDataReceived;

        // Act.
        _serialPortHandlerMock.Raise(m => m.SerialDataReceived += null, eventArgs!);

        // Assert.
        Assert.Equal(expectedInvocation, isEventRaised);

        void onDataReceived() => isEventRaised = true;
    }

    [Fact]
    public void SerialPortDataHandler_Dispose()
    {
        // Arrange. / Act.
        using (var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object))
        {
        }

        // Assert.
        _serialPortHandlerMock.VerifyRemove(m => m.SerialDataReceived -= It.IsAny<SerialDataReceivedEventHandler>());
        _serialPortHandlerMock.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void FlushIncomingData()
    {
        // Arrange.
        using var handler = new SerialPortDataHandler(_serialPortHandlerMock.Object);

        // Act.
        handler.FlushIncomingData();

        // Assert.
        _serialPortHandlerMock.Verify(m => m.DiscardInBuffer(), Times.Once);
    }
}
