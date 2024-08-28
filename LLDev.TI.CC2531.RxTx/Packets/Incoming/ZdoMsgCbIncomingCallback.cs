using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
internal sealed class ZdoMsgCbIncomingCallback : IncomingPacket, IIncomingPacket
{
    public ushort SrcAddr { get; }
    public bool IsBroadcast { get; }
    public ZigBeeClusterId ClusterId { get; }
    public byte SeqNum { get; }
    public ushort MacDstAddr { get; }
    public byte[] MsgData { get; }
    public ZdoMsgCbIncomingCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x09)
    {
        SrcAddr = GetUShort(Data[1], Data[0]);
        IsBroadcast = Data[2] != 0;
        ClusterId = (ZigBeeClusterId)GetUShort(Data[4], Data[3]);
        SeqNum = Data[6];
        MacDstAddr = GetUShort(Data[8], Data[7]);
        MsgData = Data[9..];
    }
}
