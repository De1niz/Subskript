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
    [Authorize(AuthenticationSchemes = "Bearer")] // 🔐 JWT Token required
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
                .Include(u => u.Subscriptions) // ✅ Kullanıcının aboneliklerini getir
                .Select(u => new 
                { 
                    u.Id, 
                    u.Username, 
                    u.Email,
                    Subscriptions = u.Subscriptions.Select(s => new 
                    {
                        s.Platform,
                        s.Status,
                        s.StartDate,
                        s.EndDate,
                        s.Amount
                    }).ToList()
                })
                .ToList();

            return Ok(users);
        }

        // 🔹 DELETE /api/admin/users/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userToDelete = _context.Users
                .Include(u => u.Subscriptions) // ✅ Kullanıcının aboneliklerini de getir
                .FirstOrDefault(u => u.Id == id);

            if (userToDelete == null)
            {
                _logger.LogWarning($"User ID {id} not found.");
                return NotFound(new { message = "User not found." });
            }

            try
            {
                // Kullanıcının varsa aboneliklerini sil
                if (userToDelete.Subscriptions != null && userToDelete.Subscriptions.Any())
                {
                    _context.Subscriptions.RemoveRange(userToDelete.Subscriptions);
                }

                _context.Users.Remove(userToDelete);
                _context.SaveChanges();
                _logger.LogInformation($"User ID {id} and their subscriptions deleted.");
                return NoContent(); // ✅ 204 No Content
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the user." });
            }
        }
    }
}
