using Microsoft.Extensions.Options;

namespace SmartMeterServer.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IOptions<Settings.CookieSettings> options,
            Abstract.Services.IUserService userService, 
            Abstract.Services.ISecurityService securityService, 
            Abstract.Services.ICurrentUserService currentUserService)
        {
            string? accesstoken = context.Request.Cookies[options.Value.AccessToken];

            if (!string.IsNullOrWhiteSpace(accesstoken))
            {
                var jwtToken = securityService.ValidateToken(accesstoken);
                if (jwtToken != null)
                {
                    int userId = int.Parse(jwtToken.Claims.First(c => c.Type == securityService.IdClaim).Value);
                    context.Items[currentUserService.CurrentUserContextItem] = userService.GetById(userId);
                }
            }

            await _next(context);
        }
    }
}
