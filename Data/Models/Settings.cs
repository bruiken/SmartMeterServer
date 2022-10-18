using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Setting
    {
        [Key]
        [Required]
        public SettingKey Key { get; set; }

        [Required]
        public string Value { get; set; }
    }

    public enum SettingKey
    {
        RabbitMQHostname,
        RabbitMQPort,
    }

    static class SettingMeta
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
