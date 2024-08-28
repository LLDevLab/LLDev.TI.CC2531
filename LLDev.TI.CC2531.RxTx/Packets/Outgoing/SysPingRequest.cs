﻿using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class SysPingRequest() : OutgoingPacket(ZToolCmdType.SysPingReq, 0), IOutgoingPacket
{
    protected override byte[] Data => [];
}
