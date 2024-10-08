﻿using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoActiveEpResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoActiveEpResponse : IncomingPacket, IZdoActiveEpResponse
{
    public ZToolPacketStatus Status { get; }
    public ZdoActiveEpResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
