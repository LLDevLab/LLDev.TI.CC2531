using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class MessageCallbackMethodsCacheServiceTests
{
    [Fact]
    public void GetAndRemove_KeyNotFound_ThrowsMessageException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new MessageCallbackMethodsCacheService();

        // Act. / Assert.
        Assert.Throws<MessageException>(() => service.GetAndRemove(CmdType));
    }

    [Fact]
    public void EntireCacheTest()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var actionExecutionCount = 0;

        var service = new MessageCallbackMethodsCacheService();

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
        action(null);

        Assert.Equal(1, actionExecutionCount);
    }
}
