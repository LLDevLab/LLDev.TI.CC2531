using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class ZdoIeeeAddrCallback : IncomingPacket, IIncomingPacket
{
    private const int BytesInUshort = 2;

    public ZToolPacketStatus Status { get; }

    /// <summary>
    /// 64 bit IEEE address of source device.
    /// </summary>
    public ulong IeeeAddr { get; }

    /// <summary>
    /// Specifies the short network address of responding device.
    /// </summary>
    public ushort NwkAddr { get; }

    /// <summary>
    /// Specifies the starting index into the list of associated devices for this report.
    /// </summary>
    public byte StartIndex { get; }

    /// <summary>
    /// Specifies the number of associated devices.
    /// </summary>
    public byte NumAssocDev { get; }

    /// <summary>
    /// Contains the list of network address for associated devices. This list can be a
    /// partial list if the entire list doesn’t fit into a packet.If it is a partial list, the starting
    /// index is StartIndex.
    /// </summary>
    public ushort[] AssocDevList { get; }

    public ZdoIeeeAddrCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x0d)
    {
        Status = (ZToolPacketStatus)Data[0];
        IeeeAddr = GetLittleEndianULong(Data[1..9]);
        NwkAddr = GetUShort(Data[10], Data[9]);
        StartIndex = Data[11];
        NumAssocDev = Data[12];

        if (NumAssocDev > 0)
        {
            var assocDevListByteArray = Data[13..];

            var arrayLen = assocDevListByteArray.Length / BytesInUshort;

            AssocDevList = new ushort[arrayLen];

            var assocDevListArrayIdx = 0;

            for (var i = 0; i < arrayLen; i++)
            {
                AssocDevList[i] = GetUShort(assocDevListByteArray[assocDevListArrayIdx + 1], assocDevListByteArray[assocDevListArrayIdx]);

                assocDevListArrayIdx += BytesInUshort;
            }
        }
        else
        {
            AssocDevList = [];
        }
    }
}
