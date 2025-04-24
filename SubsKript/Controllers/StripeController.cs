using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using SubsKript.Data;
using SubsKript.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubModel = SubsKript.Models.Subscription; 

namespace SubsKript.Controllers
{
    [Route("")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly StripeService _stripeService;

        public StripeController(AppDbContext context, StripeService stripeService)
        {
            _context = context;
            _stripeService = stripeService;
            StripeConfiguration.ApiKey = "sk_test_51R49gSLwf2wYz1lQq77S0ms4pCVKGfanIGMkH3YISSvlNcCKdq1fHn0H8CgCF5YCqM9YHUpxlx3ecLY6D6fNkOTD003dZ7WFMw"; // Stripe Secret Key buraya
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromQuery] int userId, [FromQuery] string platform)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(platform))
                return BadRequest("Kullanıcı veya platform bilgisi eksik.");

            var session = await _stripeService.CreateCheckoutSessionAsync(userId, platform);
            return Ok(new { url = session.Url });
        }

        [HttpGet("api/subscription/{userId}/{platform}")]
        public IActionResult GetSubscriptionInfo(int userId, string platform)
        {
            var subscription = _context.Subscriptions
                .FirstOrDefault(s => s.UserId == userId && s.Platform.ToLower() == platform.ToLower());

            if (subscription == null)
                return NotFound(new { message = "Abonelik bulunamadı." });

            return Ok(new
            {
                subscription.Platform,
                subscription.StartDate,
                subscription.EndDate,
                subscription.Status,
                subscription.Amount
            });
        }

        
        
        
        
        
        [HttpGet("success")]
        public async Task<IActionResult> Success([FromQuery] string session_id)
        {
            var session = await _stripeService.GetSessionAsync(session_id);
            var metadata = session.Metadata;

            if (!metadata.ContainsKey("userId") || !metadata.ContainsKey("platform"))
                return BadRequest("Stripe metadata eksik.");

            int userId = int.Parse(metadata["userId"]);
            string platform = metadata["platform"];
            string subscriptionId = session.SubscriptionId;

            Stripe.Subscription stripeSub = await _stripeService.GetStripeSubscriptionAsync(subscriptionId);
            var amount = stripeSub.Items.Data[0].Price.UnitAmountDecimal ?? 0;

            var newSub = new SubModel
            {
                UserId = userId,
                Platform = platform,
                Status = stripeSub.Status,
                StartDate = stripeSub.CurrentPeriodStart.ToShortDateString(),
                EndDate = stripeSub.CurrentPeriodEnd.ToShortDateString(),
                Amount = (decimal)(amount / 100)
            };

            _context.Subscriptions.Add(newSub);
            await _context.SaveChangesAsync();

            return Redirect($"/success.html?session_id={session_id}");
        }

        [HttpGet("checkout-session")]
        public async Task<IActionResult> GetCheckoutSession([FromQuery] string sessionId)
        {
            var session = await _stripeService.GetSessionAsync(sessionId);
            var subscription = await _stripeService.GetStripeSubscriptionAsync(session.SubscriptionId);

            var metadata = session.Metadata;
            string name = session.CustomerDetails?.Name ?? "Bilinmiyor";
            string platform = metadata.ContainsKey("platform") ? metadata["platform"] : "Bilinmiyor";
            long? amountTotal = session.AmountTotal;

            long start = ((DateTimeOffset)subscription.CurrentPeriodStart).ToUnixTimeSeconds();
            long end = ((DateTimeOffset)subscription.CurrentPeriodEnd).ToUnixTimeSeconds();

            return Ok(new
            {
                customer_name = name,
                platform = platform,
                amount_total = amountTotal,
                start_date = start,
                end_date = end
            });
        }

        public class PlanRequest
        {
            public string Plan { get; set; }
        }
    }
}
