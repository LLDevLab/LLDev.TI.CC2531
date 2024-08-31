using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class AwaitedPacketCacheServiceTests
{
    [Fact]
    public void GetAndRemove_KeyNotFound_ThrowsPacketException()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var service = new AwaitedPacketCacheService();

        // Act. / Assert.
        var exception = Assert.Throws<PacketException>(() => service.GetAndRemove(CmdType));

        // Assert.
        Assert.Equal($"Packet of type {CmdType} do not awaited", exception.Message);
    }

    [Fact]
    public void EntireCacheTest()
    {
        // Arrange.
        const ZToolCmdType CmdType = ZToolCmdType.ZbGetDeviceInfoReq;

        var actionExecutionCount = 0;

        var service = new AwaitedPacketCacheService();

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
        action(null!);

        Assert.Equal(1, actionExecutionCount);
    }
}
