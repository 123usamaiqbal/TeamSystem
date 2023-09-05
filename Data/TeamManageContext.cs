using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamManageSystem.Models.Account;

namespace TeamManageSystem.Data
{
    public class TeamManageContext : DbContext
    {

        public TeamManageContext(DbContextOptions<TeamManageContext> options)
           : base(options)
        {
        }
        public DbSet<TeamManageSystem.Models.Account.User> User { get; set; } = default!;
        public DbSet<TeamManageSystem.Models.Account.TMembers> TMembers { get; set; } = default!;
        public DbSet<TeamManageSystem.Models.Account.Sprint> Sprint { get; set; } = default!;
        public DbSet<TeamManageSystem.Models.Account.Rating> Rating { get; set; } = default!;
        public DbSet<TeamManageSystem.Models.Account.Task> Task { get; set; } = default!;
        public DbSet<TeamManageSystem.Models.Account.SprintRating> SprintRating { get; set; } = default!;

    }
}
