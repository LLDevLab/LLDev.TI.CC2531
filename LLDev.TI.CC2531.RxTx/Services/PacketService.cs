using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;

namespace LLDev.TI.CC2531.RxTx.Services;

public interface IPacketService
{
    ZToolCmdType GetPacketType(byte[] data);
}

public sealed class PacketService : IPacketService
{
    private const int MinDataSize = 4;

    public ZToolCmdType GetPacketType(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length < MinDataSize)
            throw new PacketException($"Cannot create CmdType from array, that have less than {MinDataSize} elements.");

        var cmd = (ushort)((data[2] << 8) | data[3]);

        return Enum.IsDefined(typeof(ZToolCmdType), cmd) ? (ZToolCmdType)cmd : ZToolCmdType.Unknown;
    }
}
