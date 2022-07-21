using System.Reflection;
using GClaims.Core.Filters.CustomExceptions;
using GClaims.Core.Helpers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace GClaims.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static bool IsAdded<T>(this IServiceCollection services)
    {
        return services.IsAdded(typeof(T));
    }

    public static bool IsAdded(this IServiceCollection services, Type type)
    {
        return services.Any(d => d.ServiceType == type);
    }

    public static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T)services.FirstOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;
    }

    public static T GetSingletonInstance<T>(this IServiceCollection services)
    {
        return services.GetSingletonInstanceOrNull<T>() ??
               throw new CustomException("Não foi possível encontrar o serviço singleton: " +
                                         typeof(T).AssemblyQualifiedName);
    }

    public static IServiceCollection AddDataProtectionAndRedis(this IServiceCollection services,
        AppSettingsSection appSettings)
    {
        var redisConnectionString =
            appSettings.Redis.GetConnectionString(AppSettingsSection.RedisConnectionSection.ConnectionType.Default);
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);

        services.AddDataProtection()
            .SetApplicationName("GClaims")
            .PersistKeysToStackExchangeRedis(redis, "GClaims-DataProtection-Keys");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "GClaims";
        });

        return services;
    }

    public static IServiceCollection AddDependencyInjectionByConvention(this IServiceCollection services,
        Assembly assemblie)
    {
        DependencyInjectionHelper.AddByConvention(services, assemblie);

        return services;
    }
}