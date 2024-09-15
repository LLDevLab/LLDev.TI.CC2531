﻿using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Tests.Services;
public class CmdTypeValidationServiceTests
{
    [Fact]
    public void IsResponseOrCallback()
    {
        // Arrange.
        const ZToolCmdType HighestRequestType = ZToolCmdType.SysResetReq;

        var service = new CmdTypeValidationService();

        foreach (ZToolCmdType cmdType in Enum.GetValues(typeof(ZToolCmdType)))
        {
            // Act.
            var result = service.IsResponseOrCallback(cmdType);

            // Assert.
            if ((ushort)cmdType <= (ushort)HighestRequestType)
                Assert.False(result);
            else
                Assert.True(result);
        }
    }
}