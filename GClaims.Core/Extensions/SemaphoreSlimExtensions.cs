using GClaims.Core.Types;

namespace GClaims.Core.Extensions;

public static class SemaphoreSlimExtensions
{
    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync().ConfigureAwait(false);
        return GetDispose(semaphoreSlim);
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
        return GetDispose(semaphoreSlim);
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
    {
        await semaphoreSlim.WaitAsync(millisecondsTimeout).ConfigureAwait(false);
        return GetDispose(semaphoreSlim);
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
        return GetDispose(semaphoreSlim);
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
    {
        await semaphoreSlim.WaitAsync(timeout).ConfigureAwait(false);
        return GetDispose(semaphoreSlim);
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false);
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim)
    {
        semaphoreSlim.Wait();
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(cancellationToken);
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
    {
        semaphoreSlim.Wait(millisecondsTimeout);
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout,
        CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(millisecondsTimeout, cancellationToken);
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
    {
        semaphoreSlim.Wait(timeout);
        return GetDispose(semaphoreSlim);
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(timeout, cancellationToken);
        return GetDispose(semaphoreSlim);
    }

    private static IDisposable GetDispose(this SemaphoreSlim semaphoreSlim)
    {
        return new DisposeAction(delegate { semaphoreSlim.Release(); });
    }
}