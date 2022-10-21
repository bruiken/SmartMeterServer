using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Rotom.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiAuthenticationFilterAttribute : ActionFilterAttribute, IFilterFactory
    {
        private Abstract.Services.ISecurityService _securityService;
        private Abstract.Services.ICurrentUserService _currentUserService;
        private Abstract.Services.IReaderTokenService _readerTokenService;

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new ApiAuthenticationFilterAttribute
            {
                _securityService = serviceProvider.GetService<Abstract.Services.ISecurityService>(),
                _currentUserService = serviceProvider.GetService<Abstract.Services.ICurrentUserService>(),
                _readerTokenService = serviceProvider.GetService<Abstract.Services.IReaderTokenService>(),
            };
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues headerValues = context.HttpContext.Request.Headers["Authentication"];
            if (headerValues.Count == 1)
            {
                string[] parts = headerValues.Single().Split();
                if (parts.Length == 2)
                {
                    var jwtSecurityToken = _securityService.ValidateToken(parts[1], validateLifetime: false);
                    if (jwtSecurityToken != null)
                    {
                        var claim = jwtSecurityToken.Claims.SingleOrDefault(c => c.Type == _securityService.InstallationIdClaim);
                        if (claim != null && int.TryParse(claim.Value, out int installationId))
                        {
                            int? dbInstallationToken = _readerTokenService.GetInstallationIdForToken(parts[1]);

                            if (dbInstallationToken.HasValue && dbInstallationToken.Value == installationId)
                            {
                                context.HttpContext.Items[_currentUserService.InstallationIdContextItem] = installationId;
                                return;
                            }
                        }
                    }
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
