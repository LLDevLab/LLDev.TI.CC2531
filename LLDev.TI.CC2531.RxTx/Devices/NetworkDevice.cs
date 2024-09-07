using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Devices;

internal interface INetworkDevice
{
    event DeviceAnnouncedHandler? ZigBeeDeviceAnnounced;
    event EndDeviceDataReceivedHandler? DeviceDataReceived;
}

internal sealed class NetworkDevice : INetworkDevice, IDisposable
{
    private readonly IPacketReceiverTransmitterService _packetReceiverTransmitterService;
    private readonly ILogger<NetworkDevice> _logger;

    public event DeviceAnnouncedHandler? ZigBeeDeviceAnnounced;
    public event EndDeviceDataReceivedHandler? DeviceDataReceived;

    public NetworkDevice(IPacketReceiverTransmitterService packetReceiverTransmitterService,
        ILogger<NetworkDevice> logger)
    {
        _packetReceiverTransmitterService = packetReceiverTransmitterService;
        _logger = logger;

        _packetReceiverTransmitterService.PacketReceived += OnPacketReceived;
    }

    private void OnPacketReceived(IIncomingPacket packet)
    {
        switch (packet)
        {
            case ZdoEndDeviceAnnceIndCallback zdoEndDeviceAnnceInd:
                ZigBeeDeviceAnnounced?.Invoke(new(zdoEndDeviceAnnceInd.IeeeAddr,
                    zdoEndDeviceAnnceInd.NwkAddr,
                    zdoEndDeviceAnnceInd.SrcAddr,
                    zdoEndDeviceAnnceInd.IsMainPowered,
                    zdoEndDeviceAnnceInd.IsReceiverOnWhenIdle,
                    zdoEndDeviceAnnceInd.IsSecure));
                break;
            case AfIncomingMessageCallback afIncomingMsg:
                DeviceDataReceived?.Invoke(afIncomingMsg.SrcAddr, afIncomingMsg.ClusterId, afIncomingMsg.Message);
                break;
            default:
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Packet of type {PacketType} have been ignored", packet.GetType().Name);
                break;
        }
    }

    public void Dispose() => _packetReceiverTransmitterService.PacketReceived -= OnPacketReceived;
}
