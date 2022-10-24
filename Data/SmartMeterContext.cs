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
        public DbSet<Installation> Installations { get; set; }
        public DbSet<InstallationAccess> InstallationsAccesses { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ReaderApiToken> ReaderApiTokens { get; set; }
        public DbSet<MeterData> MeterData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            UserMeta.OnModelCreating(modelBuilder);
            RefreshTokenMeta.OnModelCreating(modelBuilder);
            RoleMeta.OnModelCreating(modelBuilder);
            PermissionMeta.OnModelCreating(modelBuilder);
            InstallationMeta.OnModelCreating(modelBuilder);
            InstallationAccessMeta.OnModelCreating(modelBuilder);
            SettingMeta.OnModelCreating(modelBuilder);
            ReaderApiTokenMeta.OnModelCreating(modelBuilder);
            MeterDataMeta.OnModelCreating(modelBuilder);
        }
    }
}
