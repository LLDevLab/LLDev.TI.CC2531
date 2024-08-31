using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class AwaitedMessageCacheServiceTests
{
    [Fact]
    public void GetAndRemove_KeyNotFound_ThrowsMessageException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedMessageCacheService();

        // Act.
        var action = service.GetAndRemove(CmdType);

        // Assert.
        Assert.Null(action);
    }

    [Fact]
    public void EntireCacheTest()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var actionExecutionCount = 0;

        var service = new AwaitedMessageCacheService();

        // Act 1.
        var result1 = service.ContainsKey(CmdType);

        // Assert 1.
        Assert.False(result1);

        // Act 2.
        service.Add(CmdType, packetType => actionExecutionCount++);
        var result2 = service.ContainsKey(CmdType);

        // Assert 2.
        Assert.True(result2);

        // Act 3.
        var action = service.GetAndRemove(CmdType);
        var result3 = service.ContainsKey(CmdType);

        // Assert.
        Assert.False(result3);

        // Act 4.
        Assert.NotNull(action);
        action(null);

        Assert.Equal(1, actionExecutionCount);
    }
}
