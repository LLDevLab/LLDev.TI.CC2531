using LLDev.TI.CC2531.RxTx.Exceptions;
using System.IO.Ports;

namespace LLDev.TI.CC2531.RxTx.Handlers;

public interface ISerialPortDataHandler : IDisposable
{
    event SerialPortDataReceivedEventHandler? DataReceived;
    bool IsDataToRead { get; }
    void Write(byte[] data);
    byte[] Read(int bytesToRead);
    void FlushIncomingData();
    void Open();
    void Close();
}

public sealed class SerialPortDataHandler : ISerialPortDataHandler
{
    public event SerialPortDataReceivedEventHandler? DataReceived;
    public bool IsDataToRead => _serialPortHandler.BytesToRead > 0;

    private readonly ISerialPortHandler _serialPortHandler;

    public SerialPortDataHandler(ISerialPortHandler serialPortHandler)
    {
        _serialPortHandler = serialPortHandler;
        _serialPortHandler.SerialDataReceived += OnSerialDataReceived;
    }

    public void Write(byte[] data)
    {
        if (!_serialPortHandler.IsOpen)
            throw new SerialPortException("Serial port is closed.");

        _serialPortHandler.Write(data, 0, data.Length);
    }

    public byte[] Read(int bytesToRead)
    {
        if (!_serialPortHandler.IsOpen)
            throw new SerialPortException("Serial port is closed.");

        if (bytesToRead == 0)
            return [];

        var result = new byte[bytesToRead];

        var readBytes = 0;

        while (IsDataToRead && readBytes < bytesToRead)
            readBytes += _serialPortHandler.Read(result, readBytes, bytesToRead - readBytes);

        return result[..readBytes];
    }

    public void FlushIncomingData() => _serialPortHandler.DiscardInBuffer();

    public void Open()
    {
        if (_serialPortHandler.IsOpen)
            return;

        try
        {
            _serialPortHandler.Open();
            _serialPortHandler.DiscardInBuffer();
            _serialPortHandler.DiscardOutBuffer();
        }
        catch (FileNotFoundException ex)
        {
            throw new SerialPortException($"Serial port not found", ex);
        }
    }

    public void Close()
    {
        if (!_serialPortHandler.IsOpen)
            return;

        _serialPortHandler.Close();
    }

    private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (e.EventType != SerialData.Chars)
            return;

        DataReceived?.Invoke();
    }

    public void Dispose()
    {
        _serialPortHandler.SerialDataReceived -= OnSerialDataReceived;

        _serialPortHandler.Dispose();
    }
}
