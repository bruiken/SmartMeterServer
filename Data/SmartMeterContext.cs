using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Data
{
    public class SmartMeterContext : DbContext
    {
        public SmartMeterContext(DbContextOptions<SmartMeterContext> context) : base(context)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            UserMeta.OnModelCreating(modelBuilder);
            RefreshTokenMeta.OnModelCreating(modelBuilder);
        }
    }
}
