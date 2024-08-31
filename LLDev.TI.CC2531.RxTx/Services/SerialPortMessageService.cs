using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using Microsoft.Extensions.Options;

namespace LLDev.TI.CC2531.RxTx.Services;

// ISerialPortMessageService will be added through DI and should not inherit IDisposable interface
internal interface ISerialPortMessageService
{
    event MessageReceivedHandler? MessageReceived;
    void Send(IOutgoingPacket packet);
    T SendAndWaitForResponse<T>(IOutgoingPacket packet, ZToolCmdType responseType) where T : IIncomingPacket;
}

internal sealed class SerialPortMessageService : ISerialPortMessageService, IDisposable
{
    public event MessageReceivedHandler? MessageReceived;
    private event MessageReceivedHandler? AwaitedMessageReceived;

    private readonly ISerialPortMessageHandler _messageHandler;
    private readonly IAwaitedPacketCacheService _awaitedMessageCacheService;
    private readonly SerialPortMessageServiceConfig _config;

    public SerialPortMessageService(ISerialPortMessageHandler messageHandler,
        IAwaitedPacketCacheService awaitedMessageCacheService,
        IOptions<SerialPortMessageServiceConfig> options)
    {
        _messageHandler = messageHandler;
        _awaitedMessageCacheService = awaitedMessageCacheService;
        _config = options.Value;

        _messageHandler.MessageReceivedAsync += OnMessageReceivedInternal;
    }

    public void Send(IOutgoingPacket packet) => _messageHandler.Send(packet);

    public T SendAndWaitForResponse<T>(IOutgoingPacket packet, ZToolCmdType responseType) where T : IIncomingPacket
    {
        if (_awaitedMessageCacheService.Contains(responseType))
            throw new PacketException($"Already awaiting packet {responseType}");

        var timeout = _config.MessageWaitTimeoutMs;

        IIncomingPacket? response = null;

        using var manualResetEvent = new ManualResetEventSlim(false);

        AwaitedMessageReceived += OnAwaitedMessageReceived;

        _awaitedMessageCacheService.Add(responseType);

        _messageHandler.Send(packet);

        try
        {
            return !manualResetEvent.Wait(timeout)
                ? throw new TimeoutException($"Cannot receive response within specified duretion {timeout} ms")
                : response is null
                ? throw new PacketException("Awaited packet cannot be null")
                : response is not T result
                ? throw new PacketException($"Cannot cast packet of type {response.GetType()} to {nameof(T)}")
                : result;
        }
        finally
        {
            AwaitedMessageReceived -= OnAwaitedMessageReceived;
        }

        void OnAwaitedMessageReceived(IIncomingPacket packet)
        {
            if (packet.CmdType != responseType)
                return;

            _awaitedMessageCacheService.Remove(responseType);
            response = packet;
            manualResetEvent.Set();
        }
    }

    private void OnMessageReceivedInternal(IIncomingPacket packet)
    {
        if (_awaitedMessageCacheService.Contains(packet.CmdType))
            AwaitedMessageReceived?.Invoke(packet);
        else
            MessageReceived?.Invoke(packet);
    }

    // Should not dispose _messageHandler, it is added through DI
    public void Dispose() => _messageHandler.MessageReceivedAsync -= OnMessageReceivedInternal;
}
