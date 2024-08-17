using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using Microsoft.Extensions.Options;

namespace LLDev.TI.CC2531.RxTx.Tests.Handlers;

public class SerialPortHandlerTests
{
    [Fact]
    public void SerialPortHandlerConstructor_PortNameIsNull_ThrowsSerialPortException()
    {
        // Arrange.
        var options = Options.Create(new SerialPortHandlerConfig
        {
            PortName = null!
        });

        // Act. / Assert.
        var exception = Assert.Throws<SerialPortException>(() => new SerialPortHandler(options));

        Assert.Equal("Cannot initialize SerialPort instance. Port name is null or empty", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void SerialPortHandlerConstructor_PortNameIsEmptyOrWhiteSpace_ThrowsSerialPortException(string portName)
    {
        // Arrange.
        var options = Options.Create(new SerialPortHandlerConfig
        {
            PortName = portName
        });

        // Act. / Assert.
        var exception = Assert.Throws<SerialPortException>(() => new SerialPortHandler(options));

        Assert.Equal("Cannot initialize SerialPort instance. Port name is null or empty", exception.Message);
    }
}