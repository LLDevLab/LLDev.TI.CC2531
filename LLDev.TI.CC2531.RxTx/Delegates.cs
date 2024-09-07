using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;

namespace LLDev.TI.CC2531.RxTx;

internal delegate void SerialPortDataReceivedEventHandler();
internal delegate void MessageReceivedHandler(IIncomingPacket packet);
public delegate Task DeviceAnnouncedHandler(DiviceAnnounceInfo diviceAnnounceInfo);
