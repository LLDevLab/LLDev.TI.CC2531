namespace LLDev.TI.CC2531.RxTx.Exceptions;
public sealed class MessageException : ExceptionBase
{
    public MessageException()
    {
    }

    public MessageException(string message) : base(message)
    {
    }

    public MessageException(string message, Exception inner) : base(message, inner)
    {
    }
}
