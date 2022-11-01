using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Installation
    {
        public int Id { get; set; }

        [Required]
        public string LocationId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string RabbitMQUsername { get; set; }

        [Required]
        public string RabbitMQPassword { get; set; }

        [Required]
        public string RabbitMQExchange { get; set; }

        [Required]
        public string RabbitMQVHost { get; set; }

        [Required]
        public string Timezone { get; set; }
    }

    static class InstallationMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Installation>()
                .HasAlternateKey(m => m.LocationId);
            
            modelBuilder.Entity<Installation>()
                .HasAlternateKey(m => m.Name);
        }
    }
}
