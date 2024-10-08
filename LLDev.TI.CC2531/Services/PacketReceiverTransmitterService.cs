﻿using LLDev.TI.CC2531.Configs;
using LLDev.TI.CC2531.Enums;
using LLDev.TI.CC2531.Exceptions;
using LLDev.TI.CC2531.Handlers;
using LLDev.TI.CC2531.Packets.Incoming;
using LLDev.TI.CC2531.Packets.Outgoing;
using Microsoft.Extensions.Options;

namespace LLDev.TI.CC2531.Services;

// IPacketReceiverTransmitterService will be added through DI and should not inherit IDisposable interface
internal interface IPacketReceiverTransmitterService
{
    event PacketReceivedHandler? PacketReceived;
    void Send(IOutgoingPacket packet);
    void Initialize();
    T SendAndWaitForResponse<T>(IOutgoingPacket packet, ZToolCmdType responseType) where T : IIncomingPacket;
}

internal sealed class PacketReceiverTransmitterService : IPacketReceiverTransmitterService, IDisposable
{
    public event PacketReceivedHandler? PacketReceived;
    private event PacketReceivedHandler? PacketMessageReceived;

    private readonly IPacketHandler _messageHandler;
    private readonly ICmdTypeValidationService _cmdTypeValidationService;
    private readonly IAwaitedPacketCacheService _awaitedMessageCacheService;
    private readonly PacketReceiverTransmitterServiceConfig _config;

    public PacketReceiverTransmitterService(IPacketHandler messageHandler,
        ICmdTypeValidationService cmdTypeValidationService,
        IAwaitedPacketCacheService awaitedMessageCacheService,
        IOptions<PacketReceiverTransmitterServiceConfig> options)
    {
        _messageHandler = messageHandler;
        _awaitedMessageCacheService = awaitedMessageCacheService;
        _cmdTypeValidationService = cmdTypeValidationService;

        _config = options.Value;

        _messageHandler.PacketReceived += OnPacketReceivedInternal;
    }

    public void Initialize() => _messageHandler.Initialize();

    public void Send(IOutgoingPacket packet) => _messageHandler.Send(packet);

    public T SendAndWaitForResponse<T>(IOutgoingPacket packet, ZToolCmdType responseType) where T : IIncomingPacket
    {
        var responseWaitTimeout = _config.WaitResponseTimeoutMs;

        if (!_cmdTypeValidationService.IsResponseOrCallback(responseType))
            throw new ArgumentException("Awaited response type is not response or callback", nameof(responseType));

        if (_awaitedMessageCacheService.Contains(responseType))
            throw new PacketException($"Already awaiting packet {responseType}");

        IIncomingPacket? response = null;

        using var manualResetEvent = new ManualResetEventSlim(false);

        PacketMessageReceived += OnAwaitedPacketReceived;

        _awaitedMessageCacheService.Add(responseType);

        _messageHandler.Send(packet);

        if (!manualResetEvent.Wait(responseWaitTimeout))
            throw new TimeoutException($"Cannot receive response within specified duration {responseWaitTimeout} ms");

        if (response is not T result)
            throw new PacketException($"Cannot cast packet to {typeof(T)}");

        PacketMessageReceived -= OnAwaitedPacketReceived;

        return result;

        void OnAwaitedPacketReceived(IIncomingPacket packet)
        {
            if (packet.CmdType != responseType)
                return;

            _awaitedMessageCacheService.Remove(responseType);
            response = packet;
            manualResetEvent.Set();
        }
    }

    private void OnPacketReceivedInternal(IIncomingPacket packet)
    {
        if (_awaitedMessageCacheService.Contains(packet.CmdType))
            PacketMessageReceived?.Invoke(packet);
        else
            PacketReceived?.Invoke(packet);
    }

    public void Dispose() => _messageHandler.PacketReceived -= OnPacketReceivedInternal;
}
