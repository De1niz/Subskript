using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// ✅ Customer işlemleri için controller (müşteri oluşturma)
[Route("api/customers")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly StripeService _stripeService;

    public CustomersController(StripeService stripeService)
    {
        _stripeService = stripeService;
    }

    // POST: /api/customers
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
