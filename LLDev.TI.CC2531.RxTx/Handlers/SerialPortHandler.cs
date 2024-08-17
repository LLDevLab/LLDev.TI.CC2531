using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Exceptions;
using Microsoft.Extensions.Options;
using System.IO.Ports;

namespace LLDev.TI.CC2531.RxTx.Handlers;

public interface ISerialPortHandler : IDisposable
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

public sealed class SerialPortHandler : ISerialPortHandler
{
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

        var readWriteTimeout = config.ReadWriteTimeoutMs;

        _serialPort = new SerialPort(config.PortName, config.BaudRate)
        {
            WriteTimeout = readWriteTimeout,
            ReadTimeout = readWriteTimeout
        };
    }

    public void DiscardInBuffer() => _serialPort.DiscardInBuffer();
    public void DiscardOutBuffer() => _serialPort.DiscardOutBuffer();
    public void Dispose() => _serialPort.Dispose();
    public void Open() => _serialPort.Open();
    public int Read(byte[] buffer, int offset, int count) => _serialPort.Read(buffer, offset, count);
    public void Write(byte[] buffer, int offset, int count) => _serialPort.Write(buffer, offset, count);
    public void Close() => _serialPort.Close();
}
