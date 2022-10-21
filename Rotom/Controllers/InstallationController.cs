using Microsoft.AspNetCore.Mvc;
using Rotom.Attributes;

namespace Rotom.Controllers
{
    [AuthenticationFilter(Permission = Abstract.Models.EPermission.CanModifyInstallations)]
    [Route("Installation")]
    public class InstallationController : Controller
    {
        public const string Name = "Installation";
        public static class Actions
        {
            public const string Index = "Index";
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string GenerateToken = "GenerateToken";
        }

        private readonly Abstract.Services.IInstallationService _installationService;
        private readonly Abstract.Services.IUserService _userService;
        private readonly Abstract.Services.IReaderTokenService _readerTokenService;

        public InstallationController(Abstract.Services.IInstallationService installationService, Abstract.Services.IUserService userService, Abstract.Services.IReaderTokenService readerTokenService)
        {
            _installationService = installationService;
            _userService = userService;
            _readerTokenService = readerTokenService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            IEnumerable<Abstract.Models.Installation> installations = _installationService.GetInstallations();
            return View(installations.Select(Util.Converters.Convert));
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult View(int id)
        {
            Abstract.Models.Installation? installation = _installationService.GetInstallation(id);
            if (installation == null)
            {
                return RedirectToAction(Actions.Index, Name);
            }
            else
            {
                return View(Util.Converters.Convert(installation));
            }
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            IEnumerable<Abstract.Models.User> users = _userService.GetUsers();
            Models.CreateInstallationModel model = Util.Converters.Convert(users);
            return View(model);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromFormAutoError] Models.CreateInstallationModel model)
        {
            Abstract.Models.Installation installation = Util.Converters.Convert(model);
            int id = _installationService.CreateInstallation(installation);
            return RedirectToAction(Actions.View, Name, new { id });
        }

        [HttpPost]
        [Route("{id}/Update")]
        public IActionResult Update([FromRoute] int id, [FromFormAutoError(ViewName = "Create")] Models.CreateInstallationModel model)
        {
            Abstract.Models.Installation installation = Util.Converters.Convert(model);
            _installationService.UpdateInstallation(installation);
            return RedirectToAction(Actions.View, Name, new { id });
        }

        [HttpGet]
        [Route("{id}/Update")]
        public IActionResult Update([FromRoute] int id)
        {
            Abstract.Models.Installation? installation = _installationService.GetInstallation(id);
            if (installation == null)
            {
                return RedirectToAction(Actions.Index, Name);
            }
            else
            {
                IEnumerable<Abstract.Models.User> users = _userService.GetUsers();
                Models.CreateInstallationModel model = Util.Converters.Convert(users, installation);
                return View("Create", model);
            }
        }

        [HttpGet]
        [Route("{id}/Delete")]
        [RedirectOnError(Action = Actions.Index, Controller = Name)]
        public IActionResult Delete([FromRoute] int id)
        {
            _installationService.DeleteInstallation(id);
            return RedirectToAction(Actions.Index, Name);
        }

        [HttpPost]
        [Route("{id}/Token")]
        public JsonResult GenerateToken([FromRoute] int id)
        {
            string token = _readerTokenService.GenerateToken(id);
            return Json(token);
        }
    }
}
