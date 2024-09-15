using LLDev.TI.CC2531.Enums;
using LLDev.TI.CC2531.Models;
using LLDev.TI.CC2531.Packets.Incoming;

namespace LLDev.TI.CC2531;

internal delegate void SerialPortDataReceivedEventHandler();
internal delegate void PacketReceivedHandler(IIncomingPacket packet);

public delegate Task EndDeviceMessageReceivedHandler(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message);
public delegate Task DeviceAnnouncedHandler(DeviceAnnounceInfo deviceAnnounceInfo);
