namespace LLDev.TI.CC2531.Exceptions;
public class ExceptionBase : Exception
{
    protected ExceptionBase()
    {
    }

    protected ExceptionBase(string message) : base(message)
    {
    }

    protected ExceptionBase(string message, Exception inner) : base(message, inner)
    {
    }
}
