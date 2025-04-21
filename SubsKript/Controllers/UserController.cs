using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using SubsKript.Data;
using SubsKript.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SubsKript.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 🔐 Giriş (POST: /api/user/login)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            if (string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password))
                return BadRequest(new { message = "Username and password are required." });

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == loginData.Username && u.Password == loginData.Password);

            if (user == null)
                return Unauthorized(new { message = "Incorrect username or password." });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                token = token
            });
        }

        // 🔐 Kayıt (POST: /api/user/register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Username) ||
                string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.Password))
            {
                return BadRequest(new { message = "Username, email and password are required." });
            }

            var userExists = _context.Users.Any(u =>
                u.Username == newUser.Username || u.Email == newUser.Email);

            if (userExists)
                return BadRequest(new { message = "This username or email is already taken." });

            var user = new User
            {
                Username = newUser.Username,
                Email = newUser.Email,
                Password = newUser.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // ✨ Stripe Customer Oluştur
            var customerService = new CustomerService();
            var stripeCustomer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = user.Email,
                Name = user.Username
            });

            user.StripeCustomerId = stripeCustomer.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered and Stripe customer created successfully." });
        }

        // 🔄 Test bağlantı (GET: /api/user/test)
        [HttpGet("test")]
        public IActionResult TestConnection()
        {
            return Ok(new { message = "Bağlantı başarılı." });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
//example
