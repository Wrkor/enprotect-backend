using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace webapi.Params.Constants
{
    public class AuthOptions
    {
        public const string ISSUER = "ASPBackend";
        public const string AUDIENCE = "VueAPP";
        const string KEY = "ljmytlkmy23421tluy412mtlposdthm123drtjudrtmh";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
