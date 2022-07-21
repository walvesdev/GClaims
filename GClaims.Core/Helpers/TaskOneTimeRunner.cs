using GClaims.Core.Extensions;

namespace GClaims.Core.Helpers;

public class TaskOneTimeRunner
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private volatile bool _runBefore;

    public async Task RunAsync(Func<Task> action)
    {
        if (_runBefore)
        {
            return;
        }

        using (await _semaphore.LockAsync().ConfigureAwait(false))
        {
            if (_runBefore)
            {
                return;
            }

            await action().ConfigureAwait(false);
            _runBefore = true;
        }
    }

    public void Run(Action action)
    {
        if (_runBefore)
        {
            return;
        }

        lock (this)
        {
            if (!_runBefore)
            {
                action();
                _runBefore = true;
            }
        }
    }
}