﻿using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoMsgCbRegisterResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoMsgCbRegisterResponse : IncomingPacket, IZdoMsgCbRegisterResponse
{
    public ZToolPacketStatus Status { get; }
    public ZdoMsgCbRegisterResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}