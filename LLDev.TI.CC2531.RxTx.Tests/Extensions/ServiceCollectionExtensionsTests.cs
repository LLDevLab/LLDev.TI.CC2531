using LLDev.TI.CC2531.RxTx.Configs;
using LLDev.TI.CC2531.RxTx.Extensions;
using LLDev.TI.CC2531.RxTx.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LLDev.TI.CC2531.RxTx.Tests.Extensions;
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddZigBeeServices()
    {
        // Arrange.
        using var host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(c =>
            {
                var settings = new Dictionary<string, string?>
                {
                    { "SerialPortHandler:PortName", "COM0" }
                };

                var builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(settings);
                c.AddConfiguration(builder.Build());
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.Configure<SerialPortHandlerConfig>(configuration.GetSection("SerialPortHandler"));

                services.AddZigBeeServices();
            })
            .Build();

        // Should not throw any exception
        var services = host.Services.GetService(typeof(INetworkHandler));
    }
}
