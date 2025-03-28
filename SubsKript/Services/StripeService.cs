using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;

public class StripeService
{
    private readonly string _secretKey;

    public StripeService(IConfiguration config)
    {
        _secretKey = config.GetValue<string>("Stripe:SecretKey");

        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new Exception("⚠ Stripe API anahtarı bulunamadı! `appsettings.json` dosyanı kontrol et.");
        }

        StripeConfiguration.ApiKey = _secretKey;
    }

    // ✅ Yeni Müşteri Oluşturma
    public async Task<string> CreateCustomer(string email, string name)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };

        var service = new CustomerService();
        var customer = await service.CreateAsync(options);
        return customer.Id;
    }

    // ✅ Yeni Abonelik Oluşturma
    public async Task<string> CreateSubscription(string customerId, string priceId)
    {
        var options = new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions { Price = priceId }
            },
            PaymentBehavior = "default_incomplete"
        };

        var service = new SubscriptionService();
        var subscription = await service.CreateAsync(options);
        return subscription.Id;
    }

    // ✅ Kullanıcının tüm aboneliklerini getir
    public async Task<List<object>> GetAllSubscriptions(string customerId)
    {
        var service = new SubscriptionService();
        var subscriptions = await service.ListAsync(new SubscriptionListOptions
        {
            Customer = customerId,
            Limit = 10
        });

        return subscriptions.Data.Select(subscription => new
        {
            Id = subscription.Id,
            Status = subscription.Status,
            Product = subscription.Items.Data.FirstOrDefault()?.Price.ProductId ?? "Bilinmiyor",
            Created = subscription.Created,
            EndDate = subscription.CurrentPeriodEnd
        }).ToList<object>();
    }

    // ✅ Mevcut Bir Aboneliğin Bilgilerini Getirme
    public async Task<object> GetSubscriptionDetails(string subscriptionId)
    {
        var service = new SubscriptionService();
        var subscription = await service.GetAsync(subscriptionId);

        if (subscription == null) return null;

        return new
        {
            Id = subscription.Id,
            Status = subscription.Status,
            CustomerId = subscription.CustomerId,
            StartDate = subscription.CurrentPeriodStart,
            EndDate = subscription.CurrentPeriodEnd,
            Amount = subscription.Items.Data.FirstOrDefault()?.Price.UnitAmountDecimal / 100 ?? 0,
            Currency = subscription.Currency.ToUpper()
        };
    }

    // ✅ Abonelik Ücretini Çekme
    public async Task<bool> ChargeSubscription(string customerId, long amount)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = "usd",
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.Status == "succeeded";
    }
}
