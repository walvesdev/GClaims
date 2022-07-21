using GClaims.Core.Attributes;
using GClaims.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Core.Types;

[DependencyInjection(Lifetime = ServiceLifetime.Transient, ExposeService = typeof(ICancellationTokenProvider))]
public class CancellationTokenProvider : ICancellationTokenProvider
{
    private CancellationToken _cancellationToken;

    public static CancellationTokenProvider Instance { get; } = new CancellationTokenProvider();

    public CancellationToken Token
    {
        get
        {
            if (_cancellationToken.IsNotNull())
            {
                return _cancellationToken;
            }

            return new CancellationTokenSource().Token;
        }
        set => _cancellationToken = value;
    }

    public IDisposable Use(CancellationToken cancellationToken)
    {
        return new DisposeAction(() =>
        {
            if (cancellationToken.IsNotNull())
            {
                Token = cancellationToken;
            }
        });
    }
}