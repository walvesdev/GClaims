using Microsoft.Extensions.Configuration;

namespace GClaims.BuildingBlocks.Infrastructure.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static T GetValueForKey<T>(this IConfiguration configuration, string key)
    {
        var appServiceIsContainer = configuration.GetValue(Definitions.AppServiceIsContainer, false);
        var settingKey = key;
        if (appServiceIsContainer)
        {
            settingKey = settingKey.Replace(":", "__");
        }

        return configuration.GetValue<T>(settingKey);
    }
}