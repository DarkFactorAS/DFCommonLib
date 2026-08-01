
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DFCommonLib.Utils
{
    public class DFCrypt
    {
        private const int IvSize = 16;

        public static string Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return string.Empty;
            }

            using var aes = Aes.Create();
            aes.Key = DeriveKey(GetEncryptionKey());
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                var plainBytes = Encoding.UTF8.GetBytes(plaintext);
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                return string.Empty;
            }

var encryptedBytes = Convert.FromBase64String(encryptedText);
if (encryptedBytes.Length <= IvSize) throw new FormatException("Encrypted payload is too short.");
using var aes = Aes.Create();
aes.Key = DeriveKey(GetEncryptionKey());
aes.IV = encryptedBytes[..IvSize];
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(encryptedBytes, IvSize, encryptedBytes.Length - IvSize);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        public static string EncryptBase64(string plaintext)
        {
            var data = Encoding.UTF8.GetBytes(plaintext ?? string.Empty);
            return Convert.ToBase64String(data);
        }

        public static string DecryptBase64(string encodedString)
        {
            if (string.IsNullOrWhiteSpace(encodedString))
            {
                return string.Empty;
            }

            var data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public static string EncryptInput(string plaintext) => EncryptBase64(plaintext);

        public static string DecryptInput(string encodedString) => DecryptBase64(encodedString);

        private static byte[] DeriveKey(string secret)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
        }

        private static string GetEncryptionKey()
        {
            var configuredKey = Environment.GetEnvironmentVariable("DFCommonLib_EncryptionKey");
            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                throw new InvalidOperationException(
                    "Encryption key is not configured. Set the 'DFCommonLib_EncryptionKey' environment variable.");
            }
            return configuredKey;
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
