using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal sealed class AfIncomingMessageCallback : IncomingPacket, IIncomingPacket
{
    /// <summary>
    /// Specifies the group ID of the device
    /// </summary>
    public ushort GroupId { get; }

    /// <summary>
    /// Specifies the cluster Id
    /// </summary>
    public ZigBeeClusterId ClusterId { get; }

    /// <summary>
    /// Specifies the ZigBee network address of the source device sending the message.
    /// </summary>
    public ushort SrcAddr { get; }

    /// <summary>
    /// Specifies the source endpoint of the message
    /// </summary>
    public byte SrcEndpoint { get; }

    /// <summary>
    /// Specifies the destination endpoint of the message
    /// </summary>
    public byte DstEndpoint { get; }
    public bool WasBroadcast { get; }

    /// <summary>
    /// Indicates the link quality measured during reception
    /// </summary>
    public byte LinkQuality { get; }
    public bool SecurityUse { get; }

    /// <summary>
    /// Specifies the timestamp of the message
    /// </summary>
    public uint TimeStamp { get; }

    /// <summary>
    /// Specifies transaction sequence number of the message
    /// </summary>
    public byte TransSeqNumber { get; }

    /// <summary>
    /// Specifies the length of the data. 
    /// </summary>
    public byte Len { get; }

    public byte[] Message { get; set; }

    public AfIncomingMessageCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x11)
    {
        GroupId = GetUShort(Data[1], Data[0]);
        ClusterId = (ZigBeeClusterId)GetUShort(Data[3], Data[2]);
        SrcAddr = GetUShort(Data[5], Data[4]);
        SrcEndpoint = Data[6];
        DstEndpoint = Data[7];
        WasBroadcast = Data[8] != 0;
        LinkQuality = Data[9];
        SecurityUse = Data[10] != 0;
        TimeStamp = GetTimeStamp();
        TransSeqNumber = Data[15];
        Len = Data[16];
        Message = GetMessage();
    }

    private byte[] GetMessage()
    {
        const int StartIdx = 17;

        if (Len == 0)
            return [];

        var endIdx = StartIdx + Len;
        return Data[StartIdx..endIdx];
    }

    private uint GetTimeStamp()
    {
        const int Idx = 11;
        uint result = 0;

        for (var i = 3; i >= 0; i--)
        {
            result <<= 8;
            result |= Data[Idx + i];
        }

        return result;
    }
}
