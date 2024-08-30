﻿using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Extensions;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Handlers;

internal interface ISerialPortMessageHandler : IDisposable
{
    event MessageReceivedHandler MessageReceivedAsync;
    void Send(IOutgoingPacket packet);
}

internal sealed class SerialPortMessageHandler : ISerialPortMessageHandler
{
    public event MessageReceivedHandler? MessageReceivedAsync;

    private readonly ISerialPortDataHandler _serialPortDataHandler;
    private readonly IPacketFactory _packetFactory;
    private readonly IPacketHeaderFactory _packetHeaderFactory;
    private readonly ILogger<SerialPortMessageHandler> _logger;

    public SerialPortMessageHandler(ISerialPortDataHandler serialPortDataHandler,
        IPacketFactory packetFactory,
        IPacketHeaderFactory packetHeaderFactory,
        ILogger<SerialPortMessageHandler> logger)
    {
        _serialPortDataHandler = serialPortDataHandler;
        _packetFactory = packetFactory;
        _packetHeaderFactory = packetHeaderFactory;
        _logger = logger;

        _serialPortDataHandler.Open();
        _serialPortDataHandler.DataReceived += OnSerialPortDataReceived;
    }

    public void Send(IOutgoingPacket packet)
    {
        ArgumentNullException.ThrowIfNull(packet);

        _serialPortDataHandler.Write(packet.ToByteArray());
    }

    private void OnSerialPortDataReceived()
    {
        while (_serialPortDataHandler.IsDataToRead)
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

                MessageReceivedAsync?.Invoke(packet);
            }
            catch (PacketHeaderException ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError(ex, "Cannot create incoming packet.");

                _serialPortDataHandler.FlushIncomingData();
            }
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
        _serialPortDataHandler.DataReceived -= OnSerialPortDataReceived;
        _serialPortDataHandler.Dispose();
    }
}
