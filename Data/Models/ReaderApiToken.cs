using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ReaderApiToken
    {
        [Required]
        [Key]
        public string Token { get; set; }

        public int InstallationId { get; set; }
    }

    public static class ReaderApiTokenMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReaderApiToken>()
                .HasAlternateKey(e => e.InstallationId);
        }
    }
}
