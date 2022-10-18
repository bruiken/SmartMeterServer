using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [Attributes.AuthenticationFilter(Permission = Abstract.Models.EPermission.CanModifySettings)]
    [Route("Setting")]
    public class SettingController : Controller
    {
        public const string Name = "Setting";
        public static class Actions
        {
            public const string Index = "Index";
        }

        private readonly Abstract.Services.ISettingsService _settingsService;

        public SettingController(Abstract.Services.ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            Abstract.Models.Settings settings = _settingsService.GetSettings();
            return View(Util.Converters.Convert(settings));
        }

        [HttpPost]
        [Route("")]
        public IActionResult Index([Attributes.FromFormAutoError] Models.SettingsModel model)
        {
            _settingsService.SaveSettings(Util.Converters.Convert(model));
            return RedirectToAction(Actions.Index, Name);
        }
    }
}
