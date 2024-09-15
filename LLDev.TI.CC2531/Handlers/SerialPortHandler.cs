using LLDev.TI.CC2531.Configs;
using LLDev.TI.CC2531.Exceptions;
using Microsoft.Extensions.Options;
using System.IO.Ports;

namespace LLDev.TI.CC2531.Handlers;

// ISerialPortHandler will be added through DI and should not inherit IDisposable interface
internal interface ISerialPortHandler
{
    event SerialDataReceivedEventHandler SerialDataReceived;
    bool IsOpen { get; }
    int BytesToRead { get; }
    void Open();
    void Close();
    void Write(byte[] buffer, int offset, int count);
    int Read(byte[] buffer, int offset, int count);
    void DiscardInBuffer();
    void DiscardOutBuffer();
}

internal sealed class SerialPortHandler : ISerialPortHandler, IDisposable
{
    private const int BaudRate = 115200;
    private const int ReadWriteTimeoutMs = 3000;

    public event SerialDataReceivedEventHandler SerialDataReceived
    {
        add => _serialPort.DataReceived += value;
        remove => _serialPort.DataReceived -= value;
    }

    public bool IsOpen => _serialPort.IsOpen;
    public int BytesToRead => _serialPort.BytesToRead;

    private readonly SerialPort _serialPort;

    public SerialPortHandler(IOptions<SerialPortHandlerConfig> options)
    {
        var config = options.Value;

        if (string.IsNullOrWhiteSpace(config.PortName))
            throw new SerialPortException("Cannot initialize SerialPort instance. Port name is null or empty");

        _serialPort = new SerialPort(config.PortName, BaudRate)
        {
            WriteTimeout = ReadWriteTimeoutMs,
            ReadTimeout = ReadWriteTimeoutMs
        };
    }

    public void DiscardInBuffer() => _serialPort.DiscardInBuffer();
    public void DiscardOutBuffer() => _serialPort.DiscardOutBuffer();
    public void Open() => _serialPort.Open();
    public int Read(byte[] buffer, int offset, int count) => _serialPort.Read(buffer, offset, count);
    public void Write(byte[] buffer, int offset, int count) => _serialPort.Write(buffer, offset, count);
    public void Close() => _serialPort.Close();
    public void Dispose() => _serialPort.Dispose();
}
