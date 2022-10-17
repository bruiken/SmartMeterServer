using Abstract.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [Attributes.AuthenticationFilter(Permission = EPermission.CanModifyUsers)]
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
        private readonly Abstract.Services.IRoleService _roleService;

        public UserController(Abstract.Services.IUserService userService, Abstract.Services.IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
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
            var roles = _roleService.GetRoles();
            Models.CreateUserModel model = new()
            {
                AvailableRoles = roles.Select(Util.Converters.Convert).ToList(),
            };

            return View(model);
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
