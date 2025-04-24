using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SubsKript.Data;
using SubsKript.Models;
using System.Linq;

namespace SubsKript.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // 🔐 JWT Token zorunlu
    public class AdminApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminApiController> _logger;

        public AdminApiController(AppDbContext context, ILogger<AdminApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 🔹 GET /api/admin/users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users
                .Select(u => new { u.Id, u.Username, u.Email })
                .ToList();

            return Ok(users);
        }

        // 🔹 DELETE /api/admin/users/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userToDelete = _context.Users.Find(id);
            if (userToDelete == null)
            {
                _logger.LogWarning($"User ID {id} not found.");
                return NotFound(new { message = "User not found." });
            }

            try
            {
                _context.Users.Remove(userToDelete);
                _context.SaveChanges();
                _logger.LogInformation($"User ID {id} deleted.");
                return NoContent(); // ✅ 204 başarı
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Hata: {ex.Message}");
                return StatusCode(500, new { message = "Kullanıcı silinirken hata oluştu." });
            }
        }
    }
}