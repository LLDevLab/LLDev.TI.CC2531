using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoStateChangedIndCallback : IIncomingPacket
{
    public ZToolZdoState State { get; }
}

internal sealed class ZdoStateChangedIndCallback : IncomingPacket, IZdoStateChangedIndCallback
{
    public ZToolZdoState State { get; }
    public ZdoStateChangedIndCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        State = (ZToolZdoState)Data[0];
    }
}
