using LLDev.TI.CC2531.RxTx.Exceptions;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IPacketHeaderFactory
{
    IPacketHeader CreatePacketHeader(byte[] data);
}

public sealed class PacketHeaderFactory : IPacketHeaderFactory
{
    public IPacketHeader CreatePacketHeader(byte[] data)
    {
        return data.Length != Constants.HeaderLength
            ? throw new PacketException($"Connot create header. Data length is not equal {Constants.HeaderLength}.")
            : data[0] != Constants.StartByte ? throw new PacketException($"Cannot create header. Invalid packet start byte '{data[0]}'.") : new PacketHeader(data);
    }
}
