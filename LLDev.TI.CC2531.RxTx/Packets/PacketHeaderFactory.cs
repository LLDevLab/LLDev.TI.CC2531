using LLDev.TI.CC2531.RxTx.Exceptions;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IPacketHeaderFactory
{
    IPacketHeader CreatePacketHeader(byte[] data);
}

public sealed class PacketHeaderFactory : IPacketHeaderFactory
{
    private const byte StartByte = 0xfe;
    private const int HeaderDataLength = 4;

    public IPacketHeader CreatePacketHeader(byte[] data)
    {
        return data.Length != HeaderDataLength
            ? throw new PacketException($"Connot create header. Data length is not equal {HeaderDataLength}.")
            : data[0] != StartByte ? throw new PacketException($"Cannot create header. Invalid packet start byte '{data[0]}'.") : new PacketHeader(data);
    }
}
