using LLDev.TI.CC2531.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.Packets.Outgoing;

internal interface IOutgoingPacket : IPacket
{
}

internal abstract class OutgoingPacket(ZToolCmdType requestType, byte dataLen) :
    Packet(new PacketHeader([StartByte, dataLen, .. BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)requestType))])), IOutgoingPacket
{
}
