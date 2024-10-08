﻿using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class SysOsalNvReadRequest : OutgoingPacket, IOutgoingPacket
{
    /// <summary>
    /// The Id of the NV item.
    /// </summary>
    public ushort Id { get; }

    /// <summary>
    /// Number of bytes offset from the beginning or the NV value.
    /// </summary>
    public byte Offset { get; }

    protected override byte[] Data { get; }

    public SysOsalNvReadRequest(ushort id, byte offset) :
        base(ZToolCmdType.SysOsalNvReadReq, 3)
    {
        Id = id;
        Offset = offset;
        Data = [.. BitConverter.GetBytes(Id), Offset];
    }
}
