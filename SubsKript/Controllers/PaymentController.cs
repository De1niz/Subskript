using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SubsKript.Services;

namespace SubsKript.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly StripeService _stripeService;

        public PaymentController(StripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost]
        public async Task<IActionResult> ChargeSubscription([FromBody] Payment model)
        {
            var success = await _stripeService.ChargeSubscription(model.CustomerId, model.Amount);

            if (success)
                return Ok(new { message = "Payment successful!" });

            return BadRequest(new { message = "Payment failed!" });
        }
    }
}