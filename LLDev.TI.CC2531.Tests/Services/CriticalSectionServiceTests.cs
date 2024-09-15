using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;

public class CriticalSectionServiceTests
{
    [Fact]
    public async Task CriticalSectionTestsAsync()
    {
        // Arrange.
        var counter = 0;

        using var manualResetEvent = new ManualResetEventSlim(false);

        var service = new CriticalSectionService();

        var task1 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            if (service.IsAllowedToEnter())
            {
                counter++;

                service.Leave();
            }
        });

        var task2 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            if (service.IsAllowedToEnter())
            {
                counter++;

                service.Leave();
            }
        });

        // Act.
        await Task.Delay(2000);

        manualResetEvent.Set();

        await Task.WhenAll(task1, task2);

        // Assert.
        Assert.Equal(1, counter);
    }
}
