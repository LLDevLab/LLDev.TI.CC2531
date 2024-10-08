﻿using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZdoNodeDescRequest : OutgoingPacket, IOutgoingPacket
{
    protected override byte[] Data { get; }

    /// <summary>
    /// Specifies NWK address of the device generating the inquiry
    /// </summary>
    public ushort DestAddr { get; }

    /// <summary>
    /// Specifies NWK address of the destination device being queried.
    /// </summary>
    public ushort NwkAddrOfInterest { get; }

    /// <param name="destAddr">Specifies NWK address of the device generating the inquiry</param>
    /// <param name="nwkAddrOfInterest">Specifies NWK address of the destination device being queried.</param>
    public ZdoNodeDescRequest(ushort destAddr, ushort nwkAddrOfInterest) :
        base(ZToolCmdType.ZdoNodeDescReq, 4)
    {
        DestAddr = destAddr;
        NwkAddrOfInterest = nwkAddrOfInterest;
        Data = [.. BitConverter.GetBytes(DestAddr), .. BitConverter.GetBytes(NwkAddrOfInterest)];
    }
}
