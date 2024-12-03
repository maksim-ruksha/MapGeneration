using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MapGeneration.BLL.Services.Auth;

public static class Jwt
{
    public const string Issuer = "Issuer";
    public const string Audience = "Audience";
    private const string Key = "mysupersecret_secretkey!1231h3iugyfho1l;3pf7tg31o8pf7g30p21875f4g03185fg10ureygf10386f4g1ew0ufyg1wuiyfg0138fyg10reufyg1e0uyrfg1038457g";
    public const int Lifetime = 30;
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}