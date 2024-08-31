namespace LLDev.TI.CC2531.RxTx.Services;

internal interface ICriticalSectionService
{
    bool IsAllowedToEnter();
    void Leave();
}

internal sealed class CriticalSectionService : ICriticalSectionService
{
    private readonly object _lock = new();

    private bool _isAllowedToEnter = true;

    public bool IsAllowedToEnter()
    {
        lock (_lock)
        {
            if (!_isAllowedToEnter)
                return false;

            _isAllowedToEnter = false;

            return true;
        }
    }

    public void Leave()
    {
        lock (_lock)
        {
            _isAllowedToEnter = true;
        }
    }
}
