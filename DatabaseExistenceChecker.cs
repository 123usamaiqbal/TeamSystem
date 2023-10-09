using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TeamManageSystem.Data;

namespace TeamManageSystem
{
    public class DatabaseExistenceChecker
    {
        private readonly TeamManageContext _context;

        public DatabaseExistenceChecker(TeamManageContext context)
        {
            _context = context;
        }

        public bool DoesDatabaseExist()
        {
            try
            {
                // Check if any migration history table exists.
                return _context.Database.GetAppliedMigrations().Any();
            }
            catch (Exception)
            {
                // Handle exceptions as needed.
                return false;
            }
        }
    }
}
