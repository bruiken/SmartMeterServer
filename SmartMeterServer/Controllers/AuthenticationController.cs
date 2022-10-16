using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SmartMeterServer.Attributes;

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
        public IActionResult SubmitLogin([FromFormAutoError(ViewName = Actions.Index)] Models.LoginModel model)
        {
            _userService.Login(model.Username, model.Password, model.RememberMe);
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
    }
}
