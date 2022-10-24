using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class MeterData
    {
        [Key]
        public int InstallationId { get; set; }

        public DateTime Time { get; set; }

        [Required]
        public decimal KwhInT1 { get; set; }
        [Required]
        public decimal KwhInT2 { get; set; }

        [Required]
        public decimal KwhOutT1 { get; set; }
        [Required] 
        public decimal KwhOutT2 { get; set; }

        [Required]
        public decimal GasReadout { get; set; }
    }

    public static class MeterDataMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeterData>()
                .HasAlternateKey(e => e.Time);
        }
    }
}
