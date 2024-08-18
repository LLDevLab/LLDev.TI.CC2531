namespace LLDev.TI.CC2531.RxTx.Exceptions;
public sealed class PacketException : ExceptionBase
{
    public PacketException()
    {
    }

    public PacketException(string message) : base(message)
    {
    }

    public PacketException(string message, Exception inner) : base(message, inner)
    {
    }
}
