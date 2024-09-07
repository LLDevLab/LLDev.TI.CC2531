using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;

namespace LLDev.TI.CC2531.RxTx;

internal delegate void SerialPortDataReceivedEventHandler();
internal delegate void PacketReceivedHandler(IIncomingPacket packet);
internal delegate Task EndDeviceDataReceivedHandler(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message);

public delegate Task DeviceAnnouncedHandler(DiviceAnnounceInfo diviceAnnounceInfo);
