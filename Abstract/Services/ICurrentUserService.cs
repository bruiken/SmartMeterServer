namespace Abstract.Services
{
    public interface ICurrentUserService
    {
        string CurrentUserContextItem { get; }

        Models.User? GetCurrentUser();

        string? GetRefreshTokenCookie();

        void SaveAccessToken(string token);

        void SaveRefreshToken(string token);

        void ClearTokenCookies();

        bool IsLoggedIn { get; }
    }
}
