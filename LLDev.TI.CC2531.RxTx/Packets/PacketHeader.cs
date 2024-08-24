using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IPacketHeader
{
    byte StartByte { get; }
    byte DataLength { get; }
    ZToolCmdType CmdType { get; }

    public byte[] ToByteArray();
}

public sealed class PacketHeader : IPacketHeader
{
    private const int HeaderLen = 4;

    public byte StartByte => _data[0];
    public byte DataLength => _data[1];
    public ZToolCmdType CmdType
    {
        get
        {
            var cmd = (ushort)((_data[2] << 8) | _data[3]);
            return Enum.IsDefined(typeof(ZToolCmdType), cmd) ? (ZToolCmdType)cmd : ZToolCmdType.Unknown;
        }
    }

    private readonly byte[] _data;

    public PacketHeader(byte[] data)
    {
        if (data.Length < HeaderLen)
            throw new PacketException($"Cannot create packet header. Packet length is less that {HeaderLen} bytes");

        _data = data[0..HeaderLen];
    }

    public byte[] ToByteArray() => _data;
}
