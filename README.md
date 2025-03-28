
# 📦 SubsKript - Subscription Tracking and Payment API

> 🚧 **This project is under active development. Features may change and improvements are ongoing. Contributions are welcome!**

SubsKript is a modern .NET Core Web API project integrated with the Stripe API, enabling users to manage subscriptions and process payments.

---

## 🚀 Features

- ✅ Create customers with Stripe
- ✅ Start a new subscription
- ✅ View subscription details
- ✅ Payment collection via Stripe (Payment Intent)
- ✅ List all active subscriptions
- ✅ Customer deletion and listing

---

## 🛠️ Technologies

- ASP.NET Core Web API
- Stripe .NET SDK
- C#
- HTML/CSS/JS Frontend
- RESTful API
---

## 📡 API Endpoints

### 🔹 Customer Operations (`CustomersController`)

| Method | URL | Description |
|-------|-----|----------|
| GET | `/api/customers` | Lists all customers |
| DELETE| `/api/customers/{id}` | Deletes a customer with a specific ID |

### 🔹 Payment Processing (`PaymentController`)

| Method | URL | Description |
|-------|-----|----------|
| POST | `/api/payment` | Collects payment from a specific customer |

---

## 💳 Stripe Service (`StripeService.cs`)

| Function | Description |
|----------|----------|
| `CreateCustomer(email, name)` | Creates new customer |
| `CreateSubscription(customerId, priceId)` | Starts new subscription |
| `GetAllSubscriptions(customerId)` | Lists all active subscriptions |
| `GetSubscriptionDetails(subscriptionId)` | Gets subscription details |
| `ChargeSubscription(customerId, amount)` | Collects payment via Stripe |

> ⚠️ Stripe API key must be defined as `Stripe:SecretKey` in `appsettings.json` file.

---

## 🔧 Installation and Operation

```bash
git clone https://github.com/kullanici-adi/subskript.git
cd subskript
dotnet restore
dotnet run
```

> The project runs at `http://localhost:4242` by default.

---

## 📁 Project Structure

├── Controllers/

│   ├── CustomersController.cs

│   ├── PaymentController.cs

│   └── SubscriptionController.cs
│

├── Models/

│   ├── Customer.cs

│   └── Payment.cs
│

├── Services/

│   └── StripeService.cs
│

├── Views/

│   ├── all_subscriptions.html

│   ├── amazon.html

│   ├── amazon.png

│   ├── apple-tv.html

│   ├── apple-tv.jpg

│   ├── Apps.html

│   ├── background.png

│   ├── dashboard.html

│   ├── espn.html

│   ├── espn.png

│   ├── forgot.html

│   ├── index.html

│   ├── netflix.html

│   ├── netflix.jpg

│   ├── register.html

│   ├── Signup.html

│   ├── spotify.html

│   ├── spotify.jpg

│   ├── subs.png

│   ├── subscribe.html

│   ├── subscription.html

│   └── success.html

│
├── Properties/

├── public/

├── appsettings.json

├── appsettings.Development.json

├── Program.cs

├── server.js

├── SubsKript.http

├── package.json

├── package-lock.json

└── README.md


---

## 👨‍💻 Contribution

If you want to contribute, you can send a pull request or create an issue. Our priority is to make the code readable, interpretable and understandable.

---

## 📬 Contact

You can contact me if you have any problems or suggestions:  
📧 denizerdogan123@icloud.com

---







