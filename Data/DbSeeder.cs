using TeamManageSystem.Migrations;
using TeamManageSystem.Models.Account;

namespace TeamManageSystem.Data
{
    public class DbSeeder
    {
        public static void SeedData(TeamManageContext context)
        {
            var user = context.User.ToList();
            if(user.Count == 0)
            {
                string hashedPasswords = BCrypt.Net.BCrypt.HashPassword("noman@321");
                var teamlead = new User
                {
                    Username = "noman321",
                    Password = hashedPasswords,
                    Role = "user",
                    Email = "noman321@gmail.com",
                    Mobile = "0000-1111-222",
                    // Set other user properties as needed
                };
                // Add the user to the Users table
                context.User.Add(teamlead);
                context.SaveChanges();
            }

        }
    }
}
