namespace LLDev.TI.CC2531.RxTx.Exceptions;
public sealed class NetworkException : ExceptionBase
{
    public NetworkException()
    {
    }

    public NetworkException(string message) :
        base(message)
    {
    }

    public NetworkException(string message, Exception inner) :
        base(message, inner)
    {
    }
}
