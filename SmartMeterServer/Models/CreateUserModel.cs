using System.ComponentModel.DataAnnotations;

namespace SmartMeterServer.Models
{
    public class CreateUserModel : ErrorViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}