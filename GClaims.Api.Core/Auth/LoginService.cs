using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using GClaims.Domain.Models.Auth.Users;
using Microsoft.IdentityModel.Tokens;

namespace GClaims.Core.Auth;

public class LoginService
{
       public SigningConfigurations SigningConfigurations { get; set; }
        public TokenConfigurations TokenConfigurations { get; set; }

        public LoginService(
              SigningConfigurations signingConfigurations
            , TokenConfigurations tokenConfigurations)
        {
            SigningConfigurations = signingConfigurations;
            TokenConfigurations = tokenConfigurations;
        }

        public AccessToken Login(AppUser User)
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
                    }//User.Role                    
                };

                credenciaisValidas = (User.Email == UserBase.Email &&
                     User.Password == UserBase.Password);
            }

            if (credenciaisValidas)
            {

                DateTime dataCriacao = DateTime.Now.AddMinutes(5);
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromMinutes(TokenConfigurations.Lifetime);

                ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(UserBase!.Email, "Login"),
                new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, UserBase.Email),
                        new Claim(ClaimTypes.Role, UserBase.Role.Name),
                        new Claim(ClaimTypes.Name, UserBase.Email),
                        new Claim("UserId", UserBase.Id.ToString()),
                }
            );             

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

                return new AccessToken()
                {
                    Authenticated = true,
                    Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    Message = "OK",
                    Token = token,
                    UserId = UserBase.Id
                };
            }
            else
            {
                return new AccessToken()
                {
                    Authenticated = false,
                    Message = "Falha ao autenticar"
                };
            }
        }
}