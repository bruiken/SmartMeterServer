using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class RefreshToken
    {
        [Key]
        [MaxLength(500)]
        public string Token { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }

    static class RefreshTokenMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
