using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubsKript.Data;
using SubsKript.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubsKript.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int userId)
        {
            var platforms = new List<string> { "Netflix", "Amazon Prime", "Spotify", "Apple TV", "S Sport Plus" };
            var subscriptions = await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            ViewBag.UserId = userId;

            return View(Tuple.Create(platforms, subscriptions));
        }

    }
}