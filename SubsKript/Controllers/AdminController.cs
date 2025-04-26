using Microsoft.AspNetCore.Mvc;
using SubsKript.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore; // ✅ Include için gerekli
using SubsKript.Models;

namespace SubsKript.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private const string AdminUser = "admin";
        private const string AdminPass = "123456";

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: /admin
        [HttpGet("")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("IsAdmin") == "true")
                return RedirectToAction("Users");

            return View("Login");
        }
        
        // 🔹 PUT: /admin/users/{id}
        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            // Kullanıcı bilgilerini güncelle
            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            _context.SaveChanges();

            return Ok(new { message = "Kullanıcı başarıyla güncellendi.", user });
        }

        // 🔹 POST: /admin/login
        [HttpPost("login")]
        public IActionResult DoLogin(string username, string password)
        {
            if (username == AdminUser && password == AdminPass)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Users");
            }

            ViewBag.Error = "Invalid admin credentials";
            return View("Login");
        }

        // 🔹 GET: /admin/users
        [HttpGet("users")]
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var users = _context.Users
                .Include(u => u.Subscriptions) // ✅ Kullanıcıların abonelikleri de dahil ediliyor
                .ToList();

            return View("Users", users);
        }

        // 🔹 GET: /admin/logout
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
