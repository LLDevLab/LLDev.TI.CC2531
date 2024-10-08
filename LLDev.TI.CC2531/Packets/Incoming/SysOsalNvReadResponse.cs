﻿using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface ISysOsalNvReadResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
    public byte Len { get; }
    public byte[] Value { get; }
}

internal sealed class SysOsalNvReadResponse : IncomingPacket, ISysOsalNvReadResponse
{
    public ZToolPacketStatus Status { get; }
    public byte Len { get; }
    public byte[] Value { get; }

    public SysOsalNvReadResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x02)
    {
        Status = (ZToolPacketStatus)Data[0];
        Len = Data[1];
        Value = GetValue();
    }

    private byte[] GetValue()
    {
        if (Len == 0)
            return [];

        var result = new byte[Len];

        for (var i = 0; i < Len; i++)
            result[i] = Data[i + 2];

        return result;
    }
}
