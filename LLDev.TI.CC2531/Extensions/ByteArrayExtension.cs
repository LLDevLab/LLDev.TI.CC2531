﻿namespace LLDev.TI.CC2531.RxTx.Extensions;
internal static class ByteArrayExtension
{
    public static string ArrayToString(this byte[] data) => BitConverter.ToString(data);
}