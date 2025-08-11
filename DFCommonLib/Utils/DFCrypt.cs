
using System.Text;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DFCommonLib.HttpApi;
using Microsoft.IdentityModel.Tokens;
using DFCommonLib.Utils;

namespace DFCommonLib.Utils
{
    public class DFCrypt
    {
        public static string EncryptInput(string plaintext)
        {
            var data = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(data);
        }

        public static string DecryptInput(string encodedString)
        {
            var data = Convert.FromBase64String(encodedString);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }

        // JWT Token Generation
        public static string GenerateJwtToken(string secret, IList<KeyValuePair<string, string>> claims)
        {
            // Enforce minimum secret length for security (e.g., 32 characters for HMAC-SHA256)
            if (string.IsNullOrEmpty(secret) || secret.Length < 32)
            {
                throw new ArgumentException("JWT secret is too short. It must be at least 32 characters long for adequate security.", nameof(secret));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims.Select(c => new System.Security.Claims.Claim(c.Key, c.Value))),
                Expires = DateTime.UtcNow.Add((expiresIn ?? TimeSpan.FromHours(1))),
                SigningCredentials = credentials
            };

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
