namespace SmartMeterServer.Models
{
    public class LoginModel : ErrorViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string RedirectTo { get; set; }
    }
}
