namespace SubsKript.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        public string Platform { get; set; }

        public string Status { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public decimal Amount { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; }
    }
}