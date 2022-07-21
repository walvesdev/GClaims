using GClaims.Core.Types;

namespace GClaims.Core.Extensions;

public static class CancellationTokenProviderExtensions
{
    public static CancellationToken FallbackToProvider(this ICancellationTokenProvider provider,
        CancellationToken prefferedValue = default)
    {
        return prefferedValue == default || prefferedValue == CancellationToken.None
            ? provider.Token
            : prefferedValue;
    }
}