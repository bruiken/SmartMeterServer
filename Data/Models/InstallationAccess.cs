using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    public class InstallationAccess
    {
        public int UserId { get; set; }

        public int InstallationId { get; set; }
    }

    static class InstallationAccessMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstallationAccess>()
                .HasKey(m => new { m.UserId, m.InstallationId });
        }
    }
}
