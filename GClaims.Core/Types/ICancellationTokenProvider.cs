namespace GClaims.Core.Types;

public interface ICancellationTokenProvider
{
    CancellationToken Token { get; }

    IDisposable Use(CancellationToken cancellationToken);
}