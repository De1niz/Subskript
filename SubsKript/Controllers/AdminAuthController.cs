using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SubsKript.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AdminAuthController(IConfiguration config)
        {
            _config = config;
        }

        // 🔹 POST: /api/admin/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            if (request.Username == "admin" && request.Password == "123456")
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid admin credentials" });
        }

        private string GenerateJwtToken(string username)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration values are missing in appsettings.json.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "admin")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // ✅ 2 saatlik geçerlilik süresi
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class AdminLoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
