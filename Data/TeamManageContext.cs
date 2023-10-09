using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamManageSystem.Models.Account;
using TeamManageSystem.Models.ClickupModels;

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
        public DbSet<ChatMessage> ChatMessages { get; set; } = default!;
        public DbSet<ClickupLists> ClickupLists { get; set; } = default!;
        public DbSet<ClickupSpace> ClickupSpace { get; set; } = default!;
        public DbSet<ClikupTask> ClikupTask { get; set; } = default!;
        public DbSet<ClickupTaskAssignee> ClickupTaskAssignee { get; set; } = default!;
        public DbSet<ClickupFolders> ClickupFolders { get; set; } = default!;
        public DbSet<StatusColors> StatusColors { get; set; } = default!;

    }
}
