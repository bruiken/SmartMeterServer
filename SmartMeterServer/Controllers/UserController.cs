using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [Attributes.AuthenticationFilter]
    [Route("User")]
    public class UserController : Controller
    {
        public const string Name = "User";
        public static class Actions
        {
            public const string Create = "Create";
            public const string Index = "Index";
            public const string DeleteUser = "DeleteUser";
        }

        private readonly Abstract.Services.IUserService _userService;

        public UserController(Abstract.Services.IUserService userService)
        {
            _userService = userService;
        }
       
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var users = _userService
                .GetUsers()
                .Select(Util.Converters.Convert);
            return View(new Models.ViewUsersModel
            {
                Users = users,
            });
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
            return RedirectToAction(Actions.Index, Name);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        [Attributes.RedirectOnError(Action = Actions.Index, Controller = Name)]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction(Actions.Index, Name);
        }
    }
}
