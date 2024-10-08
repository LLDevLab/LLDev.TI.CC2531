﻿using LLDev.TI.CC2531.Extensions;

namespace LLDev.TI.CC2531.Tests.Extensions;
public class ByteArrayExtensionTests
{
    [Fact]
    public void ArrayToString()
    {
        // Arrange.
        var arr = new byte[] { 1, 2, 3 };

        // Act.
        var result = arr.ArrayToString();

        // Assert.
        Assert.Equal("01-02-03", result);
    }
}
