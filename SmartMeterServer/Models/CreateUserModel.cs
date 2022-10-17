using System.ComponentModel.DataAnnotations;

namespace Rotom.Models
{
    public class CreateUserModel : ErrorViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}