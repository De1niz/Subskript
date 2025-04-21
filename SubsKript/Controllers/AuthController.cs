using Microsoft.AspNetCore.Mvc;
using SubsKript.Data;
using System.Linq;

namespace SubsKript.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Giriş başarılıysa yönlendir (örneğin Dashboard)
                return RedirectToAction("Index", "Dashboard", new { userId = user.Id });
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }
    }
}