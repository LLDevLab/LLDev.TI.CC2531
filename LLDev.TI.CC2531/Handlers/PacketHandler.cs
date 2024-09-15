using LLDev.TI.CC2531.Exceptions;
using LLDev.TI.CC2531.Extensions;
using LLDev.TI.CC2531.Packets;
using LLDev.TI.CC2531.Packets.Incoming;
using LLDev.TI.CC2531.Packets.Outgoing;
using LLDev.TI.CC2531.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.Handlers;

// IPacketHandler will be added through DI and should not inherit IDisposable interface
internal interface IPacketHandler
{
    event PacketReceivedHandler PacketReceived;
    void Send(IOutgoingPacket packet);
    void Initialize();
}

internal sealed class PacketHandler : IPacketHandler, IDisposable
{
    public event PacketReceivedHandler? PacketReceived;

    private readonly ISerialPortDataHandler _serialPortDataHandler;
    private readonly IPacketFactory _packetFactory;
    private readonly IPacketHeaderFactory _packetHeaderFactory;
    private readonly ICriticalSectionService _criticalSectionService;
    private readonly ILogger<PacketHandler> _logger;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;

    public PacketHandler(ISerialPortDataHandler serialPortDataHandler,
        IPacketFactory packetFactory,
        IPacketHeaderFactory packetHeaderFactory,
        ICriticalSectionService criticalSectionService,
        ILogger<PacketHandler> logger)
    {
        _serialPortDataHandler = serialPortDataHandler;
        _packetFactory = packetFactory;
        _packetHeaderFactory = packetHeaderFactory;
        _criticalSectionService = criticalSectionService;
        _logger = logger;

        _cancellationTokenSource = new();
        _cancellationToken = _cancellationTokenSource.Token;

        _serialPortDataHandler.DataReceived += OnDataReceived;
    }

    public void Initialize() => _serialPortDataHandler.Open();

    public void Send(IOutgoingPacket packet)
    {
        ArgumentNullException.ThrowIfNull(packet);

        _serialPortDataHandler.Write(packet.ToByteArray());
    }

    private void OnDataReceived()
    {
        if (!_criticalSectionService.IsAllowedToEnter())
            return;

        try
        {
            while (_serialPortDataHandler.IsDataToRead && !_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var headerData = _serialPortDataHandler.Read(Constants.HeaderLength);
                    var packetHeader = _packetHeaderFactory.CreatePacketHeader(headerData);
                    // packet data should be read until the end
                    var packetData = _serialPortDataHandler.Read(packetHeader.DataLength);
                    var checkSum = _serialPortDataHandler.Read(1);

                    var packet = CreateIncomintPacket(packetHeader, [.. packetData, .. checkSum]);

                    if (packet is null)
                        continue;

                    PacketReceived?.Invoke(packet);
                }
                catch (PacketHeaderException ex)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogError(ex, "Cannot create incoming packet.");

                    _serialPortDataHandler.FlushIncomingData();
                }
            }
        }
        finally
        {
            _criticalSectionService.Leave();
        }
    }

    private IIncomingPacket? CreateIncomintPacket(IPacketHeader packetHeader, byte[] msg)
    {
        var packet = _packetFactory.CreateIncomingPacket(packetHeader, msg);

        if (packet is null || !packet.IsPacketCorrect)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                if (packet is null)
                    _logger.LogWarning("Unknown packet type: {PacketByteArray}", msg.ArrayToString());
                else
                    _logger.LogWarning("Incorrect packet: {PacketByteArray}", msg.ArrayToString());
            }

            return null;
        }

        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace("Packet received: {Packet}", packet.ToString());

        return packet;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _serialPortDataHandler.DataReceived -= OnDataReceived;
        _cancellationTokenSource.Dispose();
    }
}
