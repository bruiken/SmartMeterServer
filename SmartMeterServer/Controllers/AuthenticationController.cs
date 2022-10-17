using Microsoft.AspNetCore.Mvc;
using Rotom.Attributes;

namespace Rotom.Controllers
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

        private IActionResult RedirectAfterLogin(string redirectTo = "")
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

        [HttpGet]
        [Route("")]
        public IActionResult Index([FromQuery] string redirectTo = "")
        {
            if (_userService.TryLoginWithRefreshToken())
            {
                return RedirectAfterLogin(redirectTo);
            }
            else
            {
                return View(new Models.LoginModel
                {
                    RedirectTo = redirectTo,
                });
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult SubmitLogin([FromFormAutoError(ViewName = Actions.Index)] Models.LoginModel model)
        {
            _userService.Login(model.Username, model.Password, model.RememberMe);
            return RedirectAfterLogin(model.RedirectTo);
        }
    }
}
