using SubsKript.Models;

namespace SubsKript.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any()) return; // Daha önce eklendiyse çık

            var users = new List<User>
            {
                new User { Username = "megan", Password = "1234" },
                new User { Username = "alex", Password = "1234" },
                new User { Username = "emma", Password = "1234" },
                new User { Username = "liam", Password = "1234" },
                new User { Username = "sophia", Password = "1234" },
                new User { Username = "jack", Password = "1234" },
                new User { Username = "ava", Password = "1234" },
                new User { Username = "oliver", Password = "1234" },
                new User { Username = "mia", Password = "1234" },
                new User { Username = "lucas", Password = "1234" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}