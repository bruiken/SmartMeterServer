using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rotom.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationFilterAttribute : ActionFilterAttribute, IFilterFactory
    {
        private Abstract.Services.ICurrentUserService _currentUserService;

        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AuthenticationFilterAttribute()
            {
                _currentUserService = serviceProvider.GetService<Abstract.Services.ICurrentUserService>(),
            };
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items[_currentUserService.CurrentUserContextItem] is not Abstract.Models.User user || user.Id <= 0)
            {
                context.Result = new RedirectToActionResult(
                    Controllers.AuthenticationController.Actions.Index,
                    Controllers.AuthenticationController.Name,
                    new { redirectTo = context.HttpContext.Request.Path }
                );
            }

            base.OnActionExecuting(context);
        }
    }
}
