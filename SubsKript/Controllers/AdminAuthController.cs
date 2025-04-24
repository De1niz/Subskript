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

        // POST: /api/admin/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "123456")
            {
                var token = GenerateJwtToken();
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid admin credentials" });
        }

        private string GenerateJwtToken()
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim("role", "admin")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
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