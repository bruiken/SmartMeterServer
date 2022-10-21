using Abstract.Models;
using Microsoft.Extensions.Options;

namespace Rotom.Services
{
    public class CurrentUserService : Abstract.Services.ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Abstract.Services.IRoleService _roleService;
        private readonly Settings.CookieSettings _cookieSettings;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IOptions<Settings.CookieSettings> options, Abstract.Services.IRoleService roleService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cookieSettings = options.Value;
            _roleService = roleService;
        }

        public string CurrentUserContextItem => "User";
        public string PermissionsContextItem => "Permissions";
        public string InstallationIdContextItem => "InstllationId";

        public bool IsLoggedIn => GetCurrentUser() != null;

        public User? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext.Items[CurrentUserContextItem] as User;
        }

        public void ClearTokenCookies()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(_cookieSettings.AccessToken);
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(_cookieSettings.RefreshToken);
        }

        public void SaveAccessToken(string token)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(_cookieSettings.AccessToken);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                _cookieSettings.AccessToken,
                token,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(_cookieSettings.AccessTokenValidityHours),
                    HttpOnly = true,
                    Secure = true,
                }
            );
        }

        public void SaveRefreshToken(string token)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(_cookieSettings.RefreshToken);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                _cookieSettings.RefreshToken,
                token,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMonths(_cookieSettings.RefreshTokenValidityMonths),
                    HttpOnly = true,
                    Secure = true,
                }
            );
        }

        public string? GetRefreshTokenCookie()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[_cookieSettings.RefreshToken];
        }

        public bool UserIsAllowed(EPermission permission)
        {
            Role? role = _httpContextAccessor.HttpContext.Items[PermissionsContextItem] as Role;
            if (role == null)
            {
                return false;
            }
            else
            {
                return _roleService.RoleIsAllowedPermission(role, permission);
            }
        }

        public int? GetCurrentInstallationId()
        {
            return _httpContextAccessor.HttpContext.Items[InstallationIdContextItem] as int?;
        }
    }
}
