using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GClaims.Core.Auth;

public class TokenConfigurations
{
    public string Audience { get; set; }

    public string Issuer { get; set; }

    public int Lifetime { get; set; }

    public string SecurityKeySecret { get; set; }
}

public class SigningConfigurations
{
    public SecurityKey Key => GetSecurityKey();

    public SigningCredentials SigningCredentials { get; private set; }

    public string SecurityKeySecret { get; set; }

    public SymmetricSecurityKey GetSecurityKey()
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKeySecret));
        
        SigningCredentials = new SigningCredentials(
            key, 
            SecurityAlgorithms.HmacSha512);
        
        return key;
    }
}