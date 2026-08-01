
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
        private const string DefaultEncryptionKey = "DarkFactor-DFCommonLib-2026-Default-Key";
        private const int NonceSizeBytes = 12;
        private const int TagSizeBytes = 16;

        public static string Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return string.Empty;
            }

            var key = DeriveKey(GetEncryptionKey());
            var nonce = new byte[NonceSizeBytes];
            RandomNumberGenerator.Fill(nonce);

            var plainBytes = Encoding.UTF8.GetBytes(plaintext);
            var cipherBytes = new byte[plainBytes.Length];
            var tag = new byte[TagSizeBytes];

            using var aesGcm = new AesGcm(key, TagSizeBytes);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            // Format: nonce (12 bytes) + tag (16 bytes) + ciphertext
            var result = new byte[NonceSizeBytes + TagSizeBytes + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSizeBytes);
            Buffer.BlockCopy(tag, 0, result, NonceSizeBytes, TagSizeBytes);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSizeBytes + TagSizeBytes, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                return string.Empty;
            }

            var encryptedBytes = Convert.FromBase64String(encryptedText);
            if (encryptedBytes.Length <= NonceSizeBytes + TagSizeBytes)
                throw new FormatException("Encrypted payload is too short.");

            var key = DeriveKey(GetEncryptionKey());
            var nonce = encryptedBytes[..NonceSizeBytes];
            var tag = encryptedBytes[NonceSizeBytes..(NonceSizeBytes + TagSizeBytes)];
            var cipherBytes = encryptedBytes[(NonceSizeBytes + TagSizeBytes)..];

            var plainBytes = new byte[cipherBytes.Length];
            using var aesGcm = new AesGcm(key, TagSizeBytes);
            aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
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
            return string.IsNullOrWhiteSpace(configuredKey) ? DefaultEncryptionKey : configuredKey;
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
