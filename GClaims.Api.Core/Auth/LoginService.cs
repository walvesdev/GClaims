using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using GClaims.Domain.Models.Auth.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GClaims.Core.Auth;

public enum AuthenticationType
{
    Jwt,
    Basic
}
public class LoginService
{
    public LoginService(
        SigningConfigurations signingConfigurations
        , TokenConfigurations tokenConfigurations)
    {
        SigningConfigurations = signingConfigurations;
        TokenConfigurations = tokenConfigurations;
    }

    public SigningConfigurations SigningConfigurations { get; set; }

    public TokenConfigurations TokenConfigurations { get; set; }

    public AccessToken Login(AppUser User, AuthenticationType authenticationType = AuthenticationType.Jwt)
    {
        var credenciaisValidas = false;

        AppUser UserBase = null;

        if (User != null && !string.IsNullOrWhiteSpace(User.Email))
        {
            UserBase = new AppUser
            {
                Email = "email@email.com",
                Password = "123",
                Role = new AppUserRole
                {
                    Name = "MASTER"
                } //User.Role                    
            };

            credenciaisValidas = User.Email == UserBase.Email &&
                                 User.Password == UserBase.Password;
        }

        if (credenciaisValidas)
        {
            var dataCriacao = DateTime.Now.AddMinutes(5);
            var dataExpiracao = dataCriacao +
                                TimeSpan.FromMinutes(TokenConfigurations.Lifetime);

            var scheme = JwtBearerDefaults.AuthenticationScheme;

            if (authenticationType == AuthenticationType.Basic)
            {
                scheme = "BasicAuthentication";
            }

            var identity = new ClaimsIdentity(
                new GenericIdentity(UserBase!.Email, scheme),
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.UniqueName, UserBase.Email),
                    new Claim(ClaimTypes.Role, UserBase.Role.Name),
                    new Claim(ClaimTypes.Name, UserBase.Email),
                    new Claim("UserId", UserBase.Id.ToString())
                }
            );

            var pricipal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(pricipal, scheme);
            
            if (authenticationType == AuthenticationType.Basic)
            {
                return new AccessToken
                {
                    Authenticated = true,
                    Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserId = UserBase.Id,
                    AuthenticationTicket = ticket
                };
            }

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = TokenConfigurations.Issuer,
                Audience = TokenConfigurations.Audience,
                SigningCredentials = SigningConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);
            
            return new AccessToken
            {
                Authenticated = true,
                Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = "OK",
                Token = token,
                UserId = UserBase.Id,
            };
        }

        return new AccessToken
        {
            Authenticated = false,
            Message = "Falha ao autenticar"
        };
    }

    public bool Validate(AppUser appUser)
    {
        var userBase = new AppUser
        {
            Email = "email@email.com",
            Password = "123",      
        };

        return  appUser.Email == userBase.Email &&
            appUser.Password == userBase.Password;
    }
}