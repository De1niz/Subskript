using Microsoft.AspNetCore.Mvc;
using SubsKript.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

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

        // 🔹 POST: /admin/login
        [HttpPost("login")]
        public IActionResult DoLogin(string username, string password)
        {
            if (username == AdminUser && password == AdminPass)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Users");
            }

            ViewBag.Error = "Geçersiz admin bilgileri";
            return View("Login");
        }

        // 🔹 GET: /admin/users
        [HttpGet("users")]
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var users = _context.Users.ToList();
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