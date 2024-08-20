using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;

public interface IOutgoingPacket : IPacket
{
}

public abstract class OutgoingPacket(ZToolCmdType requestType, byte dataLen) :
    Packet(new PacketHeader([StartByte, dataLen, GetCmd0(requestType), GetCmd1(requestType)])), IOutgoingPacket
{
    protected static byte GetLsb(ushort val) => (byte)val;
    protected static byte GetMsb(ushort val) => (byte)(val >> 8);

    private static byte GetCmd0(ZToolCmdType type) => GetMsb((ushort)type);
    private static byte GetCmd1(ZToolCmdType type) => GetLsb((ushort)type);
}
