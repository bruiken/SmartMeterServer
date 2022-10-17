using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rotom.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationFilterAttribute : ActionFilterAttribute, IFilterFactory
    {
        private Abstract.Services.ICurrentUserService _currentUserService;
        private Abstract.Services.IRoleService _roleService;

        public object? Permission { get; set; }

        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AuthenticationFilterAttribute()
            {
                _currentUserService = serviceProvider.GetService<Abstract.Services.ICurrentUserService>(),
                _roleService = serviceProvider.GetService<Abstract.Services.IRoleService>(),
                Permission = Permission
            };
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool doRedirect = false;
            bool allowRedirectUrl = true;
            if (context.HttpContext.Items[_currentUserService.CurrentUserContextItem] is not Abstract.Models.User user || user.Id <= 0)
            {
                doRedirect = true;
            }
            if (Permission is Abstract.Models.EPermission permission)
            {
                if (context.HttpContext.Items[_currentUserService.PermissionsContextItem] is not Abstract.Models.Role role 
                    || !_roleService.RoleIsAllowedPermission(role, permission))
                {
                    doRedirect = true;
                    allowRedirectUrl = false;
                }
            }

            if (doRedirect)
            {
                object? routeValues = allowRedirectUrl ? new { redirectTo = context.HttpContext.Request.Path } : null;
                context.Result = new RedirectToActionResult(
                    Controllers.AuthenticationController.Actions.Index,
                    Controllers.AuthenticationController.Name,
                    routeValues
                );
            }
        }
    }
}
