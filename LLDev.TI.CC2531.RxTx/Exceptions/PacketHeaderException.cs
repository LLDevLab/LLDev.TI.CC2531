namespace LLDev.TI.CC2531.RxTx.Exceptions;
public sealed class PacketHeaderException : ExceptionBase
{
    public PacketHeaderException()
    {
    }

    public PacketHeaderException(string message) : base(message)
    {
    }

    public PacketHeaderException(string message, Exception inner) : base(message, inner)
    {
    }
}
