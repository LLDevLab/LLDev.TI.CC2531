using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class AwaitedPacketCacheServiceTests
{
    private const int DelayMs = 1000;

    [Fact]
    public void Add_TypeAlreadyAwaited_ThrowsInvalidOperationException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        service.Add(CmdType);

        // Act. / Assert.
        Assert.Throws<InvalidOperationException>(() => service.Add(CmdType));
    }

    [Fact]
    public async Task Add_WithRaceCondition_ThrowsInvalidOperationExceptionAsync()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var counter = 0;

        using var manualResetEvent = new ManualResetEventSlim(false);

        var service = new AwaitedPacketCacheService();

        var task1 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            service.Add(CmdType);

            counter++;
        });

        var task2 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            service.Add(CmdType);

            counter++;
        });

        // Act. / Assert
        await Task.Delay(DelayMs);

        manualResetEvent.Set();

        await Assert.ThrowsAsync<InvalidOperationException>(() => Task.WhenAll(task1, task2));

        Assert.Equal(1, counter);
    }

    [Fact]
    public void Contains_TypeNotAwaited_ReturnsFalse()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        // Act.
        var result = service.Contains(CmdType);

        // Assert.
        Assert.False(result);
    }

    [Fact]
    public void Contains_TypeAwaited_ReturnsTrue()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        service.Add(CmdType);

        // Act.
        var result = service.Contains(CmdType);

        // Assert.
        Assert.True(result);
    }

    [Fact]
    public void Remove_TypeNotAwaited_DoNothing_DoNotThrowAnyExceptions()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        // Act.
        service.Remove(CmdType);
    }

    [Fact]
    public void Remove_TypeAwaited()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        // Act 1.
        var result = service.Contains(CmdType);

        // Assert 1.
        Assert.False(result);

        // Act 2.
        service.Add(CmdType);
        result = service.Contains(CmdType);

        // Assert 2.
        Assert.True(result);

        // Act 3.
        service.Remove(CmdType);
        result = service.Contains(CmdType);

        // Assert 3.
        Assert.False(result);
    }

    [Fact]
    public async Task Remove_WithRaceCondition_NotThrowAnyExceptionAsync()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        service.Add(CmdType);

        using var manualResetEvent = new ManualResetEventSlim(false);

        var task1 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            service.Remove(CmdType);
        });

        var task2 = Task.Run(() =>
        {
            manualResetEvent.Wait();

            service.Remove(CmdType);
        });

        // Act. / Assert
        await Task.Delay(DelayMs);

        manualResetEvent.Set();

        await Task.WhenAll(task1, task2);
    }
}
