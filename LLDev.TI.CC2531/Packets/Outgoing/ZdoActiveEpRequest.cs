﻿using LLDev.TI.CC2531.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZdoActiveEpRequest(ushort destAddr, ushort nwkAddrOfInterest) : OutgoingPacket(ZToolCmdType.ZdoActiveEpReq, 4), IOutgoingPacket
{
    public ushort DestAddr { get; } = destAddr;
    public ushort NwkAddrOfInterest { get; } = nwkAddrOfInterest;

    protected override byte[] Data { get; } =
        [.. BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(destAddr)), .. BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(nwkAddrOfInterest))];
}
