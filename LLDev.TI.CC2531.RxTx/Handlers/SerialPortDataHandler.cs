using LLDev.TI.CC2531.RxTx.Exceptions;
using System.IO.Ports;

namespace LLDev.TI.CC2531.RxTx.Handlers;

public interface ISerialPortDataHandler : IDisposable
{
    event SerialPortDataReceivedEventHandler? DataReceived;
    bool IsDataToRead { get; }
    bool IsOpen { get; }
    void Write(byte[] data);
    byte[] Read(int bytesToRead);
    void Open();
    void Close();
}

public sealed class SerialPortDataHandler : ISerialPortDataHandler
{
    public event SerialPortDataReceivedEventHandler? DataReceived;
    public bool IsOpen => _serialPortHandler.IsOpen;
    public bool IsDataToRead => _serialPortHandler.IsOpen && _serialPortHandler.BytesToRead > 0;

    private readonly ISerialPortHandler _serialPortHandler;

    public SerialPortDataHandler(ISerialPortHandler serialPortHandler)
    {
        _serialPortHandler = serialPortHandler;

        _serialPortHandler.SerialDataReceived += OnSerialDataReceived;
    }

    public void Write(byte[] data) => _serialPortHandler.Write(data, 0, data.Length);

    public byte[] Read(int bytesToRead)
    {
        if (bytesToRead == 0)
            return [];

        var result = new byte[bytesToRead];

        var readBytes = 0;

        while (readBytes < bytesToRead)
            readBytes += _serialPortHandler.Read(result, readBytes, bytesToRead - readBytes);

        return result;
    }

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
