using LLDev.TI.CC2531.RxTx.Exceptions;

namespace LLDev.TI.CC2531.RxTx.Packets;

internal interface IPacketHeaderFactory
{
    IPacketHeader CreatePacketHeader(byte[] data);
}

internal sealed class PacketHeaderFactory : IPacketHeaderFactory
{
    public IPacketHeader CreatePacketHeader(byte[] data)
    {
        return data.Length != Constants.HeaderLength
            ? throw new PacketHeaderException($"Data length is not equal {Constants.HeaderLength}.")
            : data[0] != Constants.StartByte ? throw new PacketHeaderException($"Invalid packet start byte '{data[0]}'.") : new PacketHeader(data);
    }
}
