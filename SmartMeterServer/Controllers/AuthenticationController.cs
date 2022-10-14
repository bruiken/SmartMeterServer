using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace SmartMeterServer.Controllers
{
    [Route("Authentication")]
    public class AuthenticationController : Controller
    {
        public const string Name = "Authentication";
        public static class Actions
        {
            public const string Index = "Index";
            public const string SubmitLogin = "SubmitLogin";
        }

        private readonly Abstract.Services.IUserService _userService;
        private readonly IStringLocalizer<AuthenticationController> _localizer;

        public AuthenticationController(Abstract.Services.IUserService userService, IStringLocalizer<AuthenticationController> localizer)
        {
            _userService = userService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index([FromQuery] string redirectTo = "")
        {
            if (_userService.TryLoginWithRefreshToken())
            {
                if (string.IsNullOrWhiteSpace(redirectTo))
                {
                    return RedirectToAction(
                        HomeController.Name,
                        HomeController.Actions.Index
                    );
                }
                else
                {
                    return Redirect(redirectTo);
                }
            }

            return View(new Models.LoginModel
            {
                RedirectTo = redirectTo,
            });
        }

        [HttpPost]
        [Route("")]
        public IActionResult SubmitLogin([FromForm] Models.LoginModel model)
        {
            if (_userService.TryLogin(model.Username, model.Password, model.RememberMe))
            {
                if (string.IsNullOrWhiteSpace(model.RedirectTo))
                {
                    return RedirectToAction(
                        HomeController.Name,
                        HomeController.Actions.Index
                    );
                }
                else
                {
                    return Redirect(model.RedirectTo);
                }
            }
            else
            {
                model.Password = string.Empty;
                model.ErrorTitle = _localizer[Resources.Controllers.AuthenticationController.Errors.CannotLogin];
                model.ErrorDetail = _localizer[Resources.Controllers.AuthenticationController.Errors.WrongUsernamePassword];
                return View(Actions.Index, model);
            }
        }
    }
}
