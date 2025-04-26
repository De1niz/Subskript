# SubsKript - Subscription Tracking and Payment API

🚧 **This project is under active development. Features may change and improvements are ongoing. Contributions are welcome!**

SubsKript is a modern web-based subscription management system built with **ASP.NET Core Web API** and integrated with the **Stripe API**. It enables users to manage digital platform subscriptions (Netflix, Spotify, Apple TV, etc.), track payment statuses, and process new subscriptions. The system also includes an admin panel for user and subscription oversight.

---

## ✨ Features

- ✅ Create Stripe customers
- ✅ Start new subscriptions
- ✅ View active subscriptions and details
- ✅ Collect payments using Stripe (Payment Intent)
- ✅ Delete or list customers
- ✅ Admin JWT-based authentication
- ✅ Responsive HTML/CSS/JS-based frontend
- ✅ Update customers, users, subscriptions via PUT methods

---

## 🛠️ Technologies Used

**Backend:**
- ASP.NET Core 8.0 Web API
- C#
- Entity Framework Core (ORM)
- Stripe .NET SDK
- MySQL Database
- JWT (JSON Web Token) Authentication

**Frontend:**
- HTML5 / CSS3 / JavaScript
- RESTful Fetch API
- LocalStorage for session management

**Tools & Utilities:**
- Swagger for API documentation/testing
- Visual Studio / JetBrains Rider
- Stripe Dashboard for real-time subscription monitoring

---

## 📡 API Endpoints

### 🔹 Customer Operations (`CustomersController`)

| Method | Endpoint                | Description                  |
|--------|--------------------------|------------------------------|
| GET    | `/api/customers`         | List all Stripe customers    |
| DELETE | `/api/customers/{id}`    | Delete a customer by ID      |
| PUT    | `/api/customers/{id}`    | Update customer details      |

### 🔹 Payment Processing (`PaymentController`)

| Method | Endpoint           | Description                            |
|--------|--------------------|----------------------------------------|
| POST   | `/api/payment`     | Collect payment from a Stripe customer |

### 🔹 Admin (`AdminController`, `AdminAuthController`)

| Method | Endpoint                | Description                      |
|--------|--------------------------|----------------------------------|
| GET    | `/admin`                 | Get admin info                   |
| GET    | `/admin/users`          | List all users                   |
| PUT    | `/admin/users/{id}`     | Update a user by ID              |
| POST   | `/admin/login`          | Admin login                      |
| GET    | `/admin/logout`         | Admin logout                     |

### 🔹 Subscription (`SubscriptionController`)

| Method | Endpoint                                      | Description                       |
|--------|-----------------------------------------------|-----------------------------------|
| GET    | `/api/subscriptions`                          | Get all subscriptions             |
| PUT    | `/api/subscriptions/{id}`                     | Update a subscription             |
| POST   | `/api/subscriptions/create-checkout-session`  | Create checkout session for Stripe|

### 🔹 Stripe (`StripeController`)

| Method | Endpoint                                      | Description                         |
|--------|-----------------------------------------------|-------------------------------------|
| POST   | `/subscribe`                                  | Start a subscription                |
| PUT    | `/api/customers/{id}`                         | Update Stripe customer              |
| GET    | `/api/subscription/{userId}/{platform}`       | Get user's subscription to platform |
| GET    | `/success`                                    | Success callback page               |
| GET    | `/checkout-session`                           | Get Stripe checkout session         |

### 🔹 User (`UserController`)

| Method | Endpoint             | Description                |
|--------|----------------------|----------------------------|
| POST   | `/api/user/login`    | User login                 |
| POST   | `/api/user/register` | User registration          |
| GET    | `/api/user/test`     | Authenticated user test    |
| PUT    | `/api/user/{id}`     | Update user info           |

### 🔹 WebHook (`WebhookController`)

| Method | Endpoint   | Description                  |
|--------|------------|------------------------------|
| POST   | `/webhook` | Handles Stripe webhook events|

---

## 💳 StripeService.cs Overview

| Function                              | Description                              |
|---------------------------------------|------------------------------------------|
| `CreateCustomer(email, name)`         | Creates a new Stripe customer            |
| `CreateSubscription(customerId, priceId)` | Starts a new subscription              |
| `GetAllSubscriptions(customerId)`     | Retrieves all active subscriptions       |
| `GetSubscriptionDetails(subscriptionId)` | Gets subscription metadata             |
| `ChargeSubscription(customerId, amount)` | Initiates a payment via Stripe        |

> ⚠️ Make sure to define your Stripe secret key in `appsettings.json` as:
```json
"Stripe": {
  "SecretKey": "your_stripe_secret_key"
}
```

---

## 🔧 Installation & Run

```bash
git clone https://github.com/Username/SubsKript.git
cd SubsKript
dotnet restore
dotnet run
```

- The backend runs at `http://localhost:5041` by default.

---

## 📁 Project Structure

```
SubsKript
└── SubsKript
    ├── Dependencies
    ├── Properties
    │   └── launchSettings.json
    ├── wwwroot
    │   ├── admin-dashboard.html
    │   ├── admin-login.html
    │   ├── admin-users.html
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
    ├── Controllers
    │   ├── AdminApiController.cs
    │   ├── AdminAuthController.cs
    │   ├── AdminController.cs
    │   ├── AuthController.cs
    │   ├── CustomersController.cs
    │   ├── DashboardController.cs
    │   ├── PaymentController.cs
    │   ├── StripeController.cs
    │   ├── SubscriptionController.cs
    │   ├── UserController.cs
    │   └── WebhookController.cs
    ├── Data
    │   ├── AppDbContext.cs
    │   └── DbInitializer.cs
    ├── Migrations
    ├── Models
    │   ├── Customer.cs
    │   ├── Payment.cs
    │   ├── RegisterRequest.cs
    │   ├── Subscription.cs
    │   └── User.cs
    ├── public
    ├── Services
    │   ├── CustomerService.cs
    │   └── StripeService.cs
    ├── Views
    │   └── Admin
    │       ├── Login.cshtml
    │       └── Users.cshtml
    ├── ApiResponse.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── package.json
    ├── package-lock.json
    ├── Program.cs
    ├── Readme.md
    ├── server.js
    └── SubsKript.http
```

---

## 🔐 Admin System

- Admins log in via `admin-login.html`
- Admin token is generated via JWT and sent with all protected requests
- Admin panel (`admin-users.html`) enables viewing and deletion of users
- Login from the top right on the login screen

---

## 🚀 Roadmap

- Subscription cancellation and updates
- User profile editing
- Admin logs and audit history
- Email notifications for expiring subscriptions
- Mobile version of the platform

---

## 👨‍💻 Contribution

We welcome contributions! Please feel free to:
- Fork the project
- Create issues for bugs or suggestions
- Submit pull requests with clean and readable code

---

## 📬 Contact

For questions, feedback, or bug reports:

**Email:** [denizerdogan123@icloud.com](mailto:denizerdogan123@icloud.com)

---

**Developed by:** Deniz Erdoğan  
**Project Name:** *SubsKript – Digital Platform Subscription Management System*
