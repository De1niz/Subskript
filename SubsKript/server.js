const express = require('express');
const path = require('path');
const app = express();
const stripe = require('stripe')('sk_test_51R49gSLwf2wYz1lQq77S0ms4pCVKGfanIGMkH3YISSvlNcCKdq1fHn0H8CgCF5YCqM9YHUpxlx3ecLY6D6fNkOTD003dZ7WFMw');
const PORT = 4242;

// 🔽 wwwroot klasörünü statik dosya olarak tanıt
app.use(express.static(path.join(__dirname, 'wwwroot')));

app.use(express.json());

// Ödeme oturumu oluşturma
app.post('/create-checkout-session', async (req, res) => {
    const { plan } = req.body;
    console.log("🟡 Gelen plan:", plan);

    const priceMap = {
        netflix:  'price_1R4BU7Lwf2wYz1lQB12rG8jR',
        amazon:'price_1R6ItKLwf2wYz1lQ9MplfjZ6',
        espn:     'price_1R4mXmLwf2wYz1lQ6j4NQ0ya',
        appletv:  'price_1R4GLlLwf2wYz1lQpDCqX7oQ',
        sspor:    'price_1R4XmFLwf2wYz1lQ6AUZadVh',
        spotify:  'price_1R6KARLwf2wYz1lQK6pFj7Q2'
    };

    const selectedPrice = priceMap[plan];

    if (!selectedPrice) {
        console.error("❌ Geçersiz plan adı:", plan);
        return res.status(400).json({ error: "Geçersiz plan adı gönderildi!" });
    }

    try {
        const session = await stripe.checkout.sessions.create({
            mode: 'subscription',
            payment_method_types: ['card'],
            line_items: [{
                price: selectedPrice,
                quantity: 1
            }],
            metadata: {
                platform: plan
            },
            success_url: 'http://localhost:4242/success.html?session_id={CHECKOUT_SESSION_ID}',
            cancel_url: `http://localhost:4242/cancel.html`
        });

        console.log("✅ Stripe Session oluşturuldu:", session.id);
        res.json({ url: session.url });

    } catch (err) {
        console.error("🔥 Stripe Hatası:", err.message);
        res.status(500).json({ error: err.message });
    }
});

// Stripe'tan session detaylarını alma (kullanıcı adı, tutar vs.)
app.get('/checkout-session', async (req, res) => {
    try {
        const session = await stripe.checkout.sessions.retrieve(req.query.sessionId, {
            expand: ['customer', 'line_items']
        });

        const customerName = session.customer_details?.name || null;
        const platform = session.metadata?.platform || null;
        const amountTotal = session.amount_total || null;
        const subscriptionId = session.subscription;

        let startDate = null;
        let endDate = null;

        if (subscriptionId) {
            const subscription = await stripe.subscriptions.retrieve(subscriptionId);
            startDate = subscription.current_period_start;
            endDate = subscription.current_period_end;
        }

        res.json({
            customer_name: customerName,
            platform: platform,
            amount_total: amountTotal,
            start_date: startDate,
            end_date: endDate
        });

    } catch (err) {
        console.error("❌ Session import error:", err.message);
        res.status(500).json({ error: "Session information not received." });
    }
});

// Sunucuyu başlat
app.listen(PORT, () => {
    console.log(`✅ Server ${PORT} port `);
});