namespace LLDev.TI.CC2531.RxTx.Extensions;
public static class ByteArrayExtension
{
    public static string ArrayToString(this byte[] data) => BitConverter.ToString(data);
}
