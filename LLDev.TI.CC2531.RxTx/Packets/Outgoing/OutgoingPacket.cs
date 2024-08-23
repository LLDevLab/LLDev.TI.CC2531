using LLDev.TI.CC2531.RxTx.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;

public interface IOutgoingPacket : IPacket
{
}

public abstract class OutgoingPacket(ZToolCmdType requestType, byte dataLen) :
    Packet(new PacketHeader([StartByte, dataLen, .. BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)requestType))])), IOutgoingPacket
{
}
