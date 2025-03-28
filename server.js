const express = require('express');
const app = express();
const stripe = require('stripe')('sk_test_51R49gSLwf2wYz1lQq77S0ms4pCVKGfanIGMkH3YISSvlNcCKdq1fHn0H8CgCF5YCqM9YHUpxlx3ecLY6D6fNkOTD003dZ7WFMw');

const PORT = 4242;

app.use(express.static('Views'));
app.use(express.json());

app.post('/create-checkout-session', async (req, res) => {
    const { plan } = req.body;
    console.log("🟡 Gelen plan:", plan);

    const priceMap = {
        netflix:  'price_1R4BU7Lwf2wYz1lQB12rG8jR',
        amazon:   'price_1R6ItKLwf2wYz1lQ9MplfjZ6',
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
            success_url: `http://localhost:${PORT}/success.html`,
            cancel_url: `http://localhost:${PORT}/cancel.html`
        });

        console.log("✅ Stripe Session oluşturuldu:", session.id);
        res.json({ url: session.url });

    } catch (err) {
        console.error("🔥 Stripe Hatası:", err.message);
        res.status(500).json({ error: err.message });
    }
});

app.listen(PORT, () => {
    console.log(`✅ Server ${PORT} `);
});
