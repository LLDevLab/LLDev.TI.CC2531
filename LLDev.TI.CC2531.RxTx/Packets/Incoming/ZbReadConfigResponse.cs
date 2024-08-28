﻿using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal sealed class ZbReadConfigResponse : IncomingPacket, IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
    public ZToolZbConfigurationId ConfigId { get; }
    public byte ConfigValueLen { get; }
    public byte[] ConfigValue { get; }

    public ZbReadConfigResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x03)
    {
        Status = (ZToolPacketStatus)Data[0];
        ConfigId = (ZToolZbConfigurationId)Data[1];
        ConfigValueLen = Data[2];
        ConfigValue = Data[3..];
    }
}
