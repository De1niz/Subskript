using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

    
[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            "whsec_555efea5f55177cba85b40eb2d9eec1e4730724613d50868b083216dd4aca566" // appsettings.json'dan da çekebilirsin
        );

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            Console.WriteLine($"✅ Ödeme tamamlandı - Session ID: {session.Id}");
            // Veritabanına ekleme işlemleri burada yapılır
        }


        return Ok();
    }
}