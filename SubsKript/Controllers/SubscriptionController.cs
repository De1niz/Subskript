using Microsoft.AspNetCore.Mvc;
using Stripe;
using SubsKript.Data;
using SubsKript.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// Stripe alias tanımı
using StripeCustomerService = Stripe.CustomerService;

namespace SubsKript.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly StripeService _stripeService;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(AppDbContext context, StripeService stripeService, ILogger<SubscriptionController> logger)
        {
            _context = context;
            _stripeService = stripeService;
            _logger = logger;
        }

        // 🔹 Abonelik bilgisi getirme (GET: /api/subscriptions?username=Ece&platform=Spotify)
        [HttpGet]
        public IActionResult GetSubscriptions([FromQuery] string username, [FromQuery] string platform)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(platform))
                return BadRequest(new { message = "Kullanıcı adı veya platform eksik." });

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound(new { message = "Kullanıcı veritabanında bulunamadı." });

            StripeConfiguration.ApiKey = "sk_test_51R49gSLwf2wYz1lQq77S0ms4pCVKGfanIGMkH3YISSvlNcCKdq1fHn0H8CgCF5YCqM9YHUpxlx3ecLY6D6fNkOTD003dZ7WFMw";
            var customerService = new StripeCustomerService();

            Customer stripeCustomer = null;

            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                stripeCustomer = customerService.Get(user.StripeCustomerId);
            }
            else
            {
                var customers = customerService.List(new CustomerListOptions
                {
                    Email = user.Email,
                    Limit = 1
                });
                stripeCustomer = customers.FirstOrDefault();
                if (stripeCustomer == null)
                    return NotFound(new { message = "Stripe'da müşteri bulunamadı." });
            }

            var subscriptionService = new SubscriptionService();
            var subscriptions = subscriptionService.List(new SubscriptionListOptions
            {
                Customer = stripeCustomer.Id
            });

            var normalizedPlatform = platform.ToLower().Replace(" ", "").Replace("+", "").Trim();

            var filtered = subscriptions
                .Where(s => s.Items.Data.Any(i =>
                    i.Price?.Product is Product product &&
                    !string.IsNullOrEmpty(product.Name) &&
                    (
                        product.Name.ToLower().Replace(" ", "").Replace("+", "").Trim().Contains(normalizedPlatform)
                        || normalizedPlatform.Contains(product.Name.ToLower().Replace(" ", "").Replace("+", "").Trim())
                    )
                ))
                .Select(s => new
                {
                    platform = platform,
                    amount = (s.Items.Data.First().Price.UnitAmountDecimal ?? 0) / 100,
                    status = s.Status,
                    startDate = s.CurrentPeriodStart.ToLocalTime().ToString("dd MMMM yyyy"),
                    endDate = s.CurrentPeriodEnd.ToLocalTime().ToString("dd MMMM yyyy")
                })
                .ToList();

            return Ok(filtered);
        }

        // 🔹 Checkout oturumu oluşturma (POST: /api/subscriptions/create-checkout-session)
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest req)
        {
            if (req == null || req.UserId <= 0 || string.IsNullOrWhiteSpace(req.Platform))
                return BadRequest(new { error = "Geçersiz istek parametreleri." });

            try
            {
                var session = await _stripeService.CreateCheckoutSessionAsync(req.UserId, req.Platform);
                return Ok(new { sessionId = session.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout oturumu oluşturulurken hata oluştu.");
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class CheckoutSessionRequest
    {
        public int UserId { get; set; }
        public string Platform { get; set; } = string.Empty;
    }
}
