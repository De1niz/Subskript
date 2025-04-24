using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Linq;

namespace SubsKript.Services
{
    public class CustomerService
    {
        private readonly string _secretKey;
        
        public CustomerService(IConfiguration config)
        {
            _secretKey = config.GetValue<string>("Stripe:SecretKey") 
                         ?? throw new Exception("Stripe Secret Key not found.");
            StripeConfiguration.ApiKey = _secretKey;
        }

        // Register a user in Stripe
        public async Task<string> CreateCustomerAsync(string email, string name)
        {
            try
            {
                var options = new CustomerCreateOptions
                {
                    Email = email,
                    Name = name
                };

                var customerService = new Stripe.CustomerService();
                var customer = await customerService.CreateAsync(options);
                return customer.Id; // Return customer ID
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while creating Stripe customer: {ex.Message}");
            }
        }

        // Retrieve customer info from Stripe
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
                throw new Exception($"Error occurred while retrieving Stripe customer info: {ex.Message}");
            }
        }

        // Update customer info in Stripe
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
                throw new Exception($"Error occurred while updating Stripe customer: {ex.Message}");
            }
        }

        // List all subscriptions for a customer
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
                throw new Exception($"Error occurred while retrieving Stripe subscriptions: {ex.Message}");
            }
        }
    }
}
