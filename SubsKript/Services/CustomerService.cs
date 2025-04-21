using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace SubsKript.Services
{
    public class CustomerService
    {
        private readonly string _secretKey;
        
        public CustomerService(IConfiguration config)
        {
            _secretKey = config.GetValue<string>("Stripe:SecretKey") 
                         ?? throw new Exception("Stripe Secret Key bulunamadı.");
            StripeConfiguration.ApiKey = _secretKey;
        }

        // Kullanıcıyı Stripe'a kaydet
        public async Task<string> CreateCustomerAsync(string email, string name)
        {
            try
            {
                // Stripe'a yeni müşteri kaydediyoruz
                var options = new CustomerCreateOptions
                {
                    Email = email,
                    Name = name
                };

                var customerService = new Stripe.CustomerService();
                var customer = await customerService.CreateAsync(options);
                return customer.Id; // Customer ID döner
            }
            catch (Exception ex)
            {
                throw new Exception($"Stripe müşteri kaydında hata oluştu: {ex.Message}");
            }
        }

        // Stripe'dan müşteri bilgilerini al
        public async Task<Stripe.Customer> GetCustomerAsync(string email)
        {
            try
            {
                var customerService = new Stripe.CustomerService();
                var customers = await customerService.ListAsync(new CustomerListOptions
                {
                    Email = email,
                    Limit = 1
                });

                return customers.Data.Count > 0 ? customers.Data[0] : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Stripe müşteri bilgileri alınırken hata oluştu: {ex.Message}");
            }
        }

        // Müşteriyi Stripe'da güncelle (örneğin, yeni özellikler ekleyebiliriz)
        public async Task<Stripe.Customer> UpdateCustomerAsync(string customerId, string newEmail, string newName)
        {
            try
            {
                var options = new CustomerUpdateOptions
                {
                    Email = newEmail,
                    Name = newName
                };

                var customerService = new Stripe.CustomerService();
                var updatedCustomer = await customerService.UpdateAsync(customerId, options);
                return updatedCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception($"Stripe müşteri güncellenirken hata oluştu: {ex.Message}");
            }
        }

        // Müşterinin aboneliklerini listele
        public async Task<List<Stripe.Subscription>> GetSubscriptionsAsync(string customerId)
        {
            try
            {
                var subscriptionService = new SubscriptionService();
                var subscriptions = await subscriptionService.ListAsync(new SubscriptionListOptions
                {
                    Customer = customerId
                });

                return subscriptions.Data.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Stripe abonelik bilgileri alınırken hata oluştu: {ex.Message}");
            }
        }
    }
}
