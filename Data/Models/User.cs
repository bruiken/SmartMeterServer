using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(60)]
        [Required]
        public string Username { get; set; }

        [MaxLength(500)]
        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(500)]
        [Required]
        public string PasswordSalt { get; set; }
    }

    static class UserMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(c => c.Username);
        }
    }
}
