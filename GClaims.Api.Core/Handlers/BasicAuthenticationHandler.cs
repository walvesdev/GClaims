using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using GClaims.Core.Auth;
using GClaims.Domain.Models.Auth.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GClaims.Core.Handlers;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public LoginService LoginService { get; }

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        LoginService loginService) : base(options, logger, encoder, clock)
    {
        LoginService = loginService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization Header not found"));
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Parameter != null)
            {
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var email = credentials[0];
                var password = credentials[1];

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return Task.FromResult(AuthenticateResult.Fail("INVALID_ACCOUNT"));
                }

                var token = LoginService.Login(new AppUser
                {
                    Email = email,
                    Password = password
                }, AuthenticationType.Basic);

                if (!token.Authenticated)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization"));
                }

                return Task.FromResult(token.AuthenticationTicket != null
                    ? AuthenticateResult.Success(token.AuthenticationTicket)
                    : AuthenticateResult.Fail("Invalid Authorization"));
            }
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail(
                "Invalid Authorization Header (Expected Base64: \"email(string):password(string)\""));
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization"));
    }
}