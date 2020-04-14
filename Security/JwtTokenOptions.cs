using System;
using Microsoft.IdentityModel.Tokens; 

namespace esquire_backend.security
{
    public class JwtTokenOptions
    {
        public static string Audience { get; } = "phlip-frontend-users";
        public static string Issuer { get; } = "phlip-backend-api";
        public static string Subject { get; } = "PHLIP";
        public static SymmetricSecurityKey Key { get; } = JwtSecurityKey.Create(Environment.GetEnvironmentVariable("securitySecret") ?? "You should really change this secret");
        public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature); 
        public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromMinutes(0);
    }
}
