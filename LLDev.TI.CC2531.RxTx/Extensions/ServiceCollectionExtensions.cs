using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Handlers;
using LLDev.TI.CC2531.RxTx.Packets;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LLDev.TI.CC2531.RxTx.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZigBeeServices(this IServiceCollection services)
    {
        services.AddSingleton<ISerialPortHandler, SerialPortHandler>();
        services.AddSingleton<ISerialPortDataHandler, SerialPortDataHandler>();
        services.AddSingleton<IPacketHeaderFactory, PacketHeaderFactory>();
        services.AddSingleton<IPacketFactory, PacketFactory>();
        services.AddSingleton<IPacketHandler, PacketHandler>();
        services.AddSingleton<ITransactionService, TransactionService>();
        services.AddSingleton<ICmdTypeValidationService, CmdTypeValidationService>();
        services.AddSingleton<IAwaitedPacketCacheService, AwaitedPacketCacheService>();
        services.AddSingleton<IPacketReceiverTransmitterService, PacketReceiverTransmitterService>();
        services.AddSingleton<INetworkCoordinator, NetworkCoordinator>();
        services.AddSingleton<INetworkCoordinatorService, NetworkCoordinatorService>();
        services.AddSingleton<INetworkDevice, NetworkDevice>();
        services.AddSingleton<INetworkHandler, NetworkHandler>();

        services.AddTransient<ICriticalSectionService, CriticalSectionService>();

        return services;
    }
}
