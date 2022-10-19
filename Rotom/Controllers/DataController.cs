using Microsoft.AspNetCore.Mvc;

namespace Rotom.Controllers
{
    [Attributes.AuthenticationFilter]
    [Route("Data")]
    public class DataController : Controller
    {
        public static class Actions
        {
            public const string Live = "Live";
        }
        public const string Name = "Data";

        private readonly Abstract.Services.IInstallationService _installationService;
        private readonly Abstract.Services.ICurrentUserService _currentUserService;
        private readonly Abstract.Services.ISettingsService _settingsService;

        public DataController(Abstract.Services.IInstallationService installationService, Abstract.Services.ICurrentUserService currentUserService, Abstract.Services.ISettingsService settingsService)
        {
            _installationService = installationService;
            _currentUserService = currentUserService;
            _settingsService = settingsService;
        }

        [HttpGet]
        [Route("Live")]
        public IActionResult Live()
        {
            int currentUserId = _currentUserService.GetCurrentUser()!.Id;
            IEnumerable<Abstract.Models.Installation> installations = _installationService.GetUserInstallations(currentUserId);
            Abstract.Models.Settings settings = _settingsService.GetSettings();

            IEnumerable<Models.LiveInstallationModel> model = installations.Select(i => Util.Converters.Convert(i, settings));
            return View(model);
        }
    }
}
