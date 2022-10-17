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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            UserMeta.OnModelCreating(modelBuilder);
            RefreshTokenMeta.OnModelCreating(modelBuilder);
            RoleMeta.OnModelCreating(modelBuilder);
            PermissionMeta.OnModelCreating(modelBuilder);
        }
    }
}
