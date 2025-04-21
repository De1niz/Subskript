using System.Collections.Generic;

namespace SubsKript.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }  // ✅ Email alanı

        public string Password { get; set; }
        
        public string StripeCustomerId { get; set; }= string.Empty;
        
        

        // İsteğe bağlı: Kullanıcının abonelikleri
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    }
}


//