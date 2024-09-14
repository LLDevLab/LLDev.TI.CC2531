using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Devices;

internal interface INetworkDevice
{
    event DeviceAnnouncedHandler? DeviceAnnouncedAsync;
    event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync;
}

internal sealed class NetworkDevice : INetworkDevice, IDisposable
{
    private readonly IPacketReceiverTransmitterService _packetReceiverTransmitterService;
    private readonly ILogger<NetworkDevice> _logger;

    public event DeviceAnnouncedHandler? DeviceAnnouncedAsync;
    public event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync;

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
            case IZdoEndDeviceAnnceIndCallback zdoEndDeviceAnnceInd:
                DeviceAnnouncedAsync?.Invoke(new(zdoEndDeviceAnnceInd.IeeeAddr,
                    zdoEndDeviceAnnceInd.NwkAddr,
                    zdoEndDeviceAnnceInd.SrcAddr,
                    zdoEndDeviceAnnceInd.IsMainPowered,
                    zdoEndDeviceAnnceInd.IsReceiverOnWhenIdle,
                    zdoEndDeviceAnnceInd.IsSecure));
                break;
            case IAfIncomingMessageCallback afIncomingMsg:
                DeviceMessageReceivedAsync?.Invoke(afIncomingMsg.SrcAddr, afIncomingMsg.ClusterId, afIncomingMsg.Message);
                break;
            default:
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Packet of type {PacketType} have been ignored", packet.GetType().Name);
                break;
        }
    }

    public void Dispose() => _packetReceiverTransmitterService.PacketReceived -= OnPacketReceived;
}
