using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class ZdoStateChangedIndCallback : IncomingPacket, IIncomingPacket
{
    public ZToolZdoState State { get; }
    public ZdoStateChangedIndCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        State = (ZToolZdoState)Data[0];
    }
}
