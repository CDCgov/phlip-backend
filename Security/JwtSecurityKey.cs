using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace esquire_backend.security 
{
    
    public static class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }

}

