using LLDev.TI.CC2531.Services;

namespace LLDev.TI.CC2531.Tests.Services;
public class TransactionServiceTests
{
    [Fact]
    public async Task GetNextTransactionIdAsync()
    {
        // Arrange.
        var transactionId1 = 0;
        var transactionId2 = 0;

        var service = new TransactionService();

        using var manualResetEvent = new ManualResetEventSlim(false);

        var task1 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            transactionId1 = service.GetNextTransactionId();
        });

        var task2 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            transactionId2 = service.GetNextTransactionId();
        });

        // Act.
        await Task.Delay(1000);

        manualResetEvent.Set();

        await Task.WhenAll(task1, task2);

        // Assert.
        Assert.NotEqual(transactionId1, transactionId2);
        Assert.True(transactionId1 is 1 or 2);
        Assert.True(transactionId2 is 1 or 2);
    }
}
