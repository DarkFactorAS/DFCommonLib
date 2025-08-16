
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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using DFCommonLib.Logger;

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
        public static string GenerateJwtToken(string secret, string audience, string issuer, uint expiresIn = 1)
        {
            // Enforce minimum secret length for security (e.g., 32 characters for HMAC-SHA256)
            if (string.IsNullOrEmpty(secret) || secret.Length < 32)
            {
                throw new ArgumentException("JWT secret is too short. It must be at least 32 characters long for adequate security.", nameof(secret));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var timeSpan = TimeSpan.FromMinutes(expiresIn);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new System.Security.Claims.ClaimsIdentity(claims.Select(c => new System.Security.Claims.Claim(c.Key, c.Value))),
                Expires = DateTime.UtcNow.Add(timeSpan),
                SigningCredentials = credentials,
                Audience = audience,
                Issuer = issuer
            };

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ValidateJwtToken(string token, string issuer, string audience, string signingKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(signingKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No leeway for expiration
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                // Log or handle the exception based on your application's needs
                DFLogger.LogOutput(DFLogLevel.EXCEPTION, "DFCrypt", $"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
