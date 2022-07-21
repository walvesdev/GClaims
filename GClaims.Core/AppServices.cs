using System.Net;
using GClaims.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GClaims.Core;

public static class AppServices
{
    public static int APP_MASTER_ID = 2;
    private static IServiceProvider? _services;

    public static IConfiguration Configuration { get; set; }

    public static IServiceCollection ServiceCollection { get; set; }

    public static string? IpAddress => GetIpAddress();

    public static IServiceProvider? ServiceProvider
    {
        get
        {
            if (_services == null)
            {
                BuildServiceProvider();
            }

            return _services;
        }
    }

    public static T GetService<T>() where T : notnull
    {
        return ServiceProvider!.GetRequiredService<T>();
    }

    private static void BuildServiceProvider()
    {
        _services = ServiceCollection.BuildServiceProvider();
    }

    public static IMediator? GetMediator()
    {
        var serviceProvider = GetService<IServiceScopeFactory>();
        return (IMediator)serviceProvider.CreateScope().ServiceProvider.GetService(typeof(IMediator));
    }

    public static string? GetIpAddress()
    {
        var ctx = GetHttpContext();
        var request = ctx?.Request;
        return request?.GetIpAddress();
    }

    public static HttpContext GetHttpContext()
    {
        var ctx = ServiceProvider!.GetRequiredService<IHttpContextAccessor>().HttpContext;
        return ctx;
    }

    public static int? GetUserId()
    {
        return GetClaim<int>("sub");
    }

    public static int? GetUnitId()
    {
        return GetClaim<int>("unit_id");
    }

    public static List<string>? GetUserRoles()
    {
        return GetClaims<List<string>>("role");
    }

    public static void SetLogErrors<T>(Exception? exception = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? message = null)
        where T : class, new()
    {
        var logger = GetService<ILogger<T>>();

        string? GetEntityName()
        {
            var generic = typeof(T);
            var instance = Activator.CreateInstance(generic);
            var typeName = instance?.GetType().Name;
            return typeName;
        }

        if (exception != null)
        {
            logger.LogError(new
            {
                userMessage = !string.IsNullOrWhiteSpace(message) ? message : null,
                statusCode,
                userInfo = GetUserInfo(),
                responseModel = GetEntityName(),
                exceptionmessage = exception.Message,
                innerMessage = exception.InnerException?.Message,
                source = exception.Source,
                stackTrace = exception.StackTrace
            }.ToString());
        }
        else
        {
            logger.LogError(new
            {
                userMessage = !string.IsNullOrWhiteSpace(message) ? message : null,
                statusCode,
                userInfo = GetUserInfo(),
                responseModel = GetEntityName()
            }.ToString());
        }
    }

    public static string? GetUserInfo()
    {
        //TODO Implementar
        return string.Empty;
    }

    public static bool IsAdmin()
    {
        return GetUserRoles()?.Contains(Constants.Roles.ADMIN) ?? false;
    }

    private static T? GetClaims<T>(string role)
    {
        var ctx = GetHttpContext();

        var user = ctx?.User;

        if (user == null)
        {
            return default;
        }

        var claims = (from claim in user.Claims
            where claim.Type == role && !string.IsNullOrWhiteSpace(claim.Value)
            select claim.Value).ToList();

        return (T)Convert.ChangeType(claims, typeof(T));
    }

    private static T? GetClaim<T>(string role)
    {
        var ctx = GetHttpContext();

        if (ctx == null)
        {
            return default;
        }

        var user = ctx.User;

        if (user == null)
        {
            return default;
        }

        var claim = user.Claims.FirstOrDefault(p => p.Type == role)?.Value;

        if (!string.IsNullOrWhiteSpace(claim))
        {
            return (T)Convert.ChangeType(claim, typeof(T));
        }

        return default;
    }
}