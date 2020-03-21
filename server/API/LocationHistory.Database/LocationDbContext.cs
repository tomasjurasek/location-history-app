using LocationHistory.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocationHistory.Database
{
    public class LocationDbContext : DbContext
    {
        public LocationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

    }
}
