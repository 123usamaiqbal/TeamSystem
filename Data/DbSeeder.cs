using TeamManageSystem.Models.Account;

namespace TeamManageSystem.Data
{
    public class DbSeeder
    {
        public static void SeedData(TeamManageContext context)
        {

            if (!context.User.Any())
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                // Create an initial user
                var user = new User
                {
                    Username = "Administrator",
                    Password = hashedPassword,
                    Role = "Admin",
                    Email = "Admin123@gmail.com",
                    Mobile = "0000-1111-222",
                    // Set other user properties as needed
                };

                // Add the user to the Users table
                context.User.Add(user);
                context.SaveChanges();
            }

        }
    }
}
