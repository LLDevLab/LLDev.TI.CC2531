﻿using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class SysOsalNvLengthRequest(ushort id) : 
    OutgoingPacket(ZToolCmdType.SysOsalNvLengthReq, 2), IOutgoingPacket
{
    public ushort Id { get; } = id;

    protected override byte[] Data => BitConverter.GetBytes(Id);
}