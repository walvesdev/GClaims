using Microsoft.AspNetCore.Http;

namespace GClaims.Core.Extensions;

public static class HttpRequestExtensions
{
    public static string? GetIpAddress(this HttpRequest request)
    {
        string ip;

        if (request.Headers.ContainsKey("X-Real-IP"))
        {
            ip = request.Headers["X-Real-IP"].LastOrDefault();
        }
        else if (request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = request.Headers["X-Forwarded-For"].LastOrDefault();
        }
        else
        {
            ip = request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        return ip?.Split(',').LastOrDefault()?.Trim();
    }
}