using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using SubsKript.Data;
using SubsKript.Models;
using Subscription = Stripe.Subscription;

namespace SubsKript.Services
{
    public class StripeService
    {
        private readonly string _secretKey;
        private readonly AppDbContext _context;

        public StripeService(IConfiguration config, AppDbContext context)
        {
            _secretKey = config.GetValue<string>("Stripe:SecretKey")
                         ?? throw new Exception("Stripe Secret Key bulunamadı.");
            StripeConfiguration.ApiKey = _secretKey;
            _context = context;
        }

        // 🔹 Dinamik Price ID al
        public async Task<string> GetPriceIdForPlatform(string platform)
        {
            var priceService = new PriceService();
            var prices = await priceService.ListAsync(new PriceListOptions
            {
                Expand = new List<string> { "data.product" },
                Active = true,
                Limit = 100
            });

            var matched = prices.Data.FirstOrDefault(p =>
                p.Product is Product product &&
                !string.IsNullOrEmpty(product.Name) &&
                product.Name.ToLower().Contains(platform.ToLower()));

            return matched?.Id;
        }

        // 🔹 Checkout oturumu + Stripe müşteri kontrol/kayıt
        public async Task<Session> CreateCheckoutSessionAsync(int userId, string platform)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            var customerService = new Stripe.CustomerService();
            Stripe.Customer customer = null;

            // ✅ StripeCustomerId kayıtlıysa kullan
            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                customer = await customerService.GetAsync(user.StripeCustomerId);
            }
            else
            {
                // Email'e göre Stripe'da arama
                var existing = await customerService.ListAsync(new CustomerListOptions
                {
                    Email = user.Email,
                    Limit = 1
                });
                customer = existing.FirstOrDefault();

                // Stripe'da da yoksa oluştur
                if (customer == null)
                {
                    var customerOptions = new CustomerCreateOptions
                    {
                        Email = user.Email,
                        Name = user.Username,
                        Description = $"Oluşturma tarihi: {DateTime.Now}"
                    };
                    customer = await customerService.CreateAsync(customerOptions);
                }

                // StripeCustomerId kaydet
                user.StripeCustomerId = customer.Id;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            // 🔹 Price ID getir
            string priceId = await GetPriceIdForPlatform(platform);
            if (string.IsNullOrEmpty(priceId))
                throw new Exception($"'{platform}' için Price ID bulunamadı!");

            // 🔹 Checkout session oluştur
            var sessionService = new SessionService();
            var sessionOptions = new SessionCreateOptions
            {
                Customer = customer.Id,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },
                Mode = "subscription",
                SuccessUrl = $"http://localhost:5041/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"http://localhost:5041/dashboard.html",
                Metadata = new Dictionary<string, string>
                {
                    { "userId", user.Id.ToString() },
                    { "platform", platform }
                }
            };

            return await sessionService.CreateAsync(sessionOptions);
        }

        // 🔹 Session bilgisi getir
        public async Task<Session> GetSessionAsync(string sessionId)
        {
            var sessionService = new SessionService();
            return await sessionService.GetAsync(sessionId);
        }

        // 🔹 Stripe abonelik bilgisi getir
        public async Task<Subscription> GetStripeSubscriptionAsync(string subscriptionId)
        {
            var subscriptionService = new SubscriptionService();
            return await subscriptionService.GetAsync(subscriptionId);
        }

        // 🔹 Manuel müşteri oluştur (opsiyonel)
        public async Task<string> CreateCustomer(string email, string name)
        {
            var customerService = new Stripe.CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = email,
                Name = name
            });

            return customer.Id;
        }

        // 🔹 Tek seferlik ödeme (opsiyonel)
        public async Task<bool> ChargeSubscription(string customerId, long amount)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                Customer = customerId,
                PaymentMethodTypes = new List<string> { "card" }
            });

            return paymentIntent.Status == "succeeded";
        }
    }
}
