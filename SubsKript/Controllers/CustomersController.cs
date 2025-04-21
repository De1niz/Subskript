using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SubsKript.Services;
using SubsKript.Models;

namespace SubsKript.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly StripeService _stripeService;

        public CustomersController(StripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest(new { message = "İsim ve e-posta boş olamaz." });
            }

            var customerId = await _stripeService.CreateCustomer(model.Email, model.Name);
            return Ok(new { customerId });
        }
    }
}