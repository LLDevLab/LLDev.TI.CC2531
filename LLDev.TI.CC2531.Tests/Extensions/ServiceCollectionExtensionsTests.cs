using LLDev.TI.CC2531.Extensions;
using LLDev.TI.CC2531.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LLDev.TI.CC2531.Tests.Extensions;
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddZigBeeServices()
    {
        // Arrange.
        const string SectionName = "SerialPortHandler";

        using var host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(c =>
            {
                var settings = new Dictionary<string, string?>
                {
                    { $"{SectionName}:PortName", "SomeRandomPort" }
                };

                var builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(settings);
                c.AddConfiguration(builder.Build());
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddZigBeeServices(configuration.GetSection(SectionName));
            })
            .Build();

        // Act.
        var service = host.Services.GetService(typeof(INetworkHandler));

        // Assert.
        Assert.NotNull(service);
    }
}
