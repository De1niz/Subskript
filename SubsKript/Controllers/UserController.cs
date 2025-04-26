using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using SubsKript.Data;
using SubsKript.Models;
using SubsKript.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using StripeCustomerService = Stripe.CustomerService;

namespace SubsKript.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly StripeService _stripeService;

        public UserController(AppDbContext context, IConfiguration config, StripeService stripeService)
        {
            _context = context;
            _config = config;
            _stripeService = stripeService;
        }

        // 🔐 Login (POST: /api/user/login)
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            if (string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password))
                return BadRequest(new ErrorResponse { Message = "Username and password are required." });

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == loginData.Username && u.Password == loginData.Password);

            if (user == null)
                return Unauthorized(new ErrorResponse { Message = "Incorrect username or password." });

            var token = GenerateJwtToken(user);

            var response = new LoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token
            };

            return Ok(response);
        }

        // 🔐 Register (POST: /api/user/register)
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Username)
             || string.IsNullOrWhiteSpace(newUser.Email)
             || string.IsNullOrWhiteSpace(newUser.Password))
            {
                return BadRequest(new ErrorResponse { Message = "Username, email, and password are required." });
            }

            var userExists = _context.Users.Any(u =>
                u.Username == newUser.Username || u.Email == newUser.Email);

            if (userExists)
                return BadRequest(new ErrorResponse { Message = "This username or email is already taken." });

            var user = new User
            {
                Username = newUser.Username,
                Email = newUser.Email,
                Password = newUser.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // ✨ Create Stripe Customer
            var customerService = new StripeCustomerService();
            var stripeCustomer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = user.Email,
                Name = user.Username
            });

            user.StripeCustomerId = stripeCustomer.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(); // returning just 200, you may add a message if needed
        }

        // 🔄 Test connection (GET: /api/user/test)
        [HttpGet("test")]
        [ProducesResponseType(typeof(TestResponse), StatusCodes.Status200OK)]
        public IActionResult TestConnection()
        {
            return Ok(new TestResponse { Message = "Connection successful." });
        }

        // 🔄 Kullanıcı Bilgisi Güncelle (PUT: /api/user/{id})
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterRequest updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ErrorResponse { Message = "User not found." });
            }

            // Güncelleme işlemleri
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Stripe müşteri bilgilerini de güncelle
            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                await _stripeService.UpdateCustomerAsync(user.StripeCustomerId, user.Username, user.Email);
            }

            return Ok(new
            {
                Message = "Kullanıcı bilgileri güncellendi.",
                user.Id,
                user.Username,
                user.Email
            });
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

        // ----- DTOs -----
        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
        }

        public class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
        }

        public class TestResponse
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}
