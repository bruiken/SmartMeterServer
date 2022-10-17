namespace Rotom.Settings
{
    public class CookieSettings
    {
        public string AccessToken { get; set; }
        public int AccessTokenValidityHours { get; set; }

        public string RefreshToken { get; set; }
        public int RefreshTokenValidityMonths { get; set; }
    }
}
