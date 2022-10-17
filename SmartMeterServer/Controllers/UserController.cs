using Microsoft.AspNetCore.Mvc;

namespace SmartMeterServer.Controllers
{
    [Attributes.AuthenticationFilter]
    [Route("User")]
    public class UserController : Controller
    {
        public const string Name = "User";
        public static class Actions
        {
            public const string Create = "Create";
        }

        private readonly Abstract.Services.IUserService _userService;

        public UserController(Abstract.Services.IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create([Attributes.FromFormAutoError] Models.CreateUserModel model)
        {
            _userService.CreateUser(Util.Converters.Convert(model));
            return RedirectToAction(HomeController.Actions.Index, HomeController.Name);
        }
    }
}
