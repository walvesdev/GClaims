using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GClaims.Core.Handlers;
using GClaims.Core.Helpers;
using GClaims.Domain.Models.Auth.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GClaims.Core.Auth;

public static class JwtConfigWebApi
{
    public static IServiceCollection AddJwtConfigWebApi(this IServiceCollection services,
        IConfiguration Configuration)
    {
        var signingConfigurations = new SigningConfigurations();
        services.AddSingleton(signingConfigurations);

        var tokenConfigurations = new TokenConfigurations();

        new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("AppSettings:TokenConfigurations"))
            .Configure(tokenConfigurations);

        signingConfigurations.SecurityKeySecret = tokenConfigurations.SecurityKeySecret;

        services.AddSingleton(tokenConfigurations);

        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null)
            .AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidateAudience = true;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidateIssuer = true;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;
                paramsValidation.ValidateLifetime = true;
                paramsValidation.ValidAlgorithms = new[] { signingConfigurations.SigningCredentials.Algorithm };
                paramsValidation.ClockSkew = TimeSpan.FromMinutes(5);

                // Any code before the first await in this delegate can run
                // synchronously, so if you have events to attach for all requests
                // attach handlers before await.
                bearerOptions.Events = new()
                {
                    OnChallenge = context =>
                    {
                        var token = context.HttpContext.Request.Headers["Authorization"];
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Ensure we always have an error and error description.
                        if (string.IsNullOrEmpty(context.Error))
                        {
                            context.Error = "invalid_token";
                        }

                        if (string.IsNullOrEmpty(context.ErrorDescription))
                        {
                            context.ErrorDescription = "This request requires a valid JWT access token to be provided";
                        }

                        // Add some extra context for expired tokens.
                        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() ==
                            typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.Response.Headers.Add("x-token-expired",
                                authenticationException!.Expires.ToString("o"));
                            context.ErrorDescription = $"The token expired on {authenticationException.Expires:o}";
                        }

                        return context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            error = context.Error,
                            error_description = context.ErrorDescription
                        }));
                    },

                    // This method is first event in authentication pipeline
                    // we have chance to wait until TokenValidationParameters
                    // is loaded.
                    OnMessageReceived = context =>
                    {
                        // Wait until token validation parameters loaded.
                        var token = context.HttpContext.Request.Headers["Authorization"];

                        if (string.IsNullOrWhiteSpace(token))
                        {
                            return Task.CompletedTask;
                        }

                        token = token.ToString().Replace("Bearer ", "");

                        try
                        {
                            // var encryptedToken = DecryptAES.DecryptStringAES(token);
                            // context.Token = encryptedToken;
                            var handler = new JwtSecurityTokenHandler();
                            var validatedToken = (SecurityToken)new JwtSecurityToken();
                            var claimsPrincipal = handler.ValidateToken(handler.ReadJwtToken(token)?.RawData,
                                paramsValidation, out validatedToken);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

        // Ativa o uso do token como forma de autorizar o acesso
        // a recursos deste projeto
        services.AddAuthorization(auth =>
        {
            var scopes = GetScopes();

            Array.ForEach(scopes, scope =>
                auth.AddPolicy(scope,
                    policy => policy.Requirements.Add(
                        new ScopeRequirement(tokenConfigurations.Issuer, scope)
                    )
                )
            );

            auth.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireClaim(
                    "AppClaims",
                    AuthPolicy.MASTER,
                    AuthPolicy.ADMIN,
                    AuthPolicy.MANAGER,
                    AuthPolicy.SELLER,
                    AuthPolicy.ECOMMERCE,
                    AuthPolicy.PARTHNER,
                    AuthPolicy.SUPORT
                )
                .Build();

            auth.AddPolicy(AuthPolicy.MASTER, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.MASTER).Build());

            auth.AddPolicy(AuthPolicy.ADMIN, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.ADMIN).Build());

            auth.AddPolicy(AuthPolicy.MANAGER, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.MANAGER).Build());

            auth.AddPolicy(AuthPolicy.SELLER, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.SELLER).Build());

            auth.AddPolicy(AuthPolicy.ECOMMERCE, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.ECOMMERCE).Build());

            auth.AddPolicy(AuthPolicy.PARTHNER, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.PARTHNER).Build());

            auth.AddPolicy(AuthPolicy.SUPORT, new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("BasicAuthentication", JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireRole(AuthPolicy.SUPORT).Build());
        });

        return services;
    }

    private static string[] GetScopes()
    {
        return new[]
        {
            "read:billing_settings",
            "update:billing_settings",
            "read:customers",
            "read:files"
        };
    }
}