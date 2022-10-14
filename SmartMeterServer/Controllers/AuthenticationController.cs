using Microsoft.AspNetCore.Mvc;

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

        public AuthenticationController(Abstract.Services.IUserService userService)
        {
            _userService = userService;
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
        public IActionResult SubmitLogin(Models.LoginModel model)
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
                return View(Actions.Index, model);
            }
        }
    }
}
