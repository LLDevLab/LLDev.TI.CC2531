using LLDev.TI.CC2531.Configs;
using LLDev.TI.CC2531.Devices;
using LLDev.TI.CC2531.Handlers;
using LLDev.TI.CC2531.Packets;
using LLDev.TI.CC2531.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LLDev.TI.CC2531.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZigBeeServices(this IServiceCollection services, IConfiguration options)
    {
        OptionsConfigurationServiceCollectionExtensions.Configure<SerialPortHandlerConfig>(services, options);
        OptionsConfigurationServiceCollectionExtensions.Configure<PacketReceiverTransmitterServiceConfig>(services, options);

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
