using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DFCommonLib.HttpApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using DFCommonLib.Utils;

namespace DFCommonLib.HttpApi
{
    [ApiController]
    [Route("[controller]")]
    public class DFRestServer : ControllerBase
    {
        private readonly ILogger<DFRestServer> _logger;

        public DFRestServer()
        {
            _logger = DFServices.GetService<ILogger<DFRestServer>>();
        }

        [HttpGet("Ping")]
        public virtual string Ping()
        {
            _logger.LogInformation("Ping received at {Time}", DateTime.UtcNow);
            return "PONG";
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid login request.");
            }

            // Simulate token generation (replace with actual implementation)
            var token = GenerateJwtToken(loginRequest.Username);

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("SecureEndpoint")]
        public IActionResult SecureEndpoint()
        {
            return Ok("This is a secure endpoint.");
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("YourSecretKeyHere"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("username", username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
