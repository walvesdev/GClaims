using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Core.Auth;

public static class AuthConfig
{
    public static IServiceCollection AddAuthConfig(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = "cookie_session";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromMinutes(60);
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
        });

        // services.AddAuthentication(
        //         CookieAuthenticationDefaults.AuthenticationScheme)
        //     .AddCookie(options =>
        //         {
        //             options.Cookie.Name = "cookie";
        //             options.LoginPath = "/Auth/Login";
        //             options.LogoutPath = "/Auth/Logout";
        //             options.AccessDeniedPath = "/Auth/AcessoNegado";
        //             options.SlidingExpiration = true;
        //             options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        //         }
        //     );

        //services.AddAuthorizationCore();

        return services;
    }
}