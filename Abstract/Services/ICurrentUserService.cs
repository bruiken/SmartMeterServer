namespace Abstract.Services
{
    public interface ICurrentUserService
    {
        string CurrentUserContextItem { get; }
        string PermissionsContextItem { get; }
        string InstallationIdContextItem { get; }

        Models.User? GetCurrentUser();

        int? GetCurrentInstallationId();

        string? GetRefreshTokenCookie();

        void SaveAccessToken(string token);

        void SaveRefreshToken(string token);

        void ClearTokenCookies();

        bool IsLoggedIn { get; }

        bool UserIsAllowed(Models.EPermission permission);

        bool CanAccessInstallation(int installationId);
    }
}
