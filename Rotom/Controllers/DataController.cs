using Microsoft.AspNetCore.Mvc;
using System;

namespace Rotom.Controllers
{
    [Attributes.AuthenticationFilter]
    [Route("Data")]
    public class DataController : Controller
    {
        public static class Actions
        {
            public const string Live = "Live";
            public const string History = "History";
            public const string HistoryPartial = "HistoryPartial";
        }
        public const string Name = "Data";

        private readonly Abstract.Services.IInstallationService _installationService;
        private readonly Abstract.Services.ICurrentUserService _currentUserService;
        private readonly Abstract.Services.ISettingsService _settingsService;
        private readonly Abstract.Services.IDataService _dataService;

        public DataController(Abstract.Services.IInstallationService installationService, Abstract.Services.ICurrentUserService currentUserService, Abstract.Services.ISettingsService settingsService, Abstract.Services.IDataService dataService)
        {
            _installationService = installationService;
            _currentUserService = currentUserService;
            _settingsService = settingsService;
            _dataService = dataService;
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

        [HttpGet]
        [Route("Installation/{installationId}/History")]
        public IActionResult History([FromRoute] int installationId)
        {
            if (!_currentUserService.CanAccessInstallation(installationId))
            {
                return Unauthorized();
            }

            IEnumerable<Abstract.Models.MeterData> data = _dataService.GetData(installationId, DateTime.Now.Date, Abstract.Models.EGraphType.Daily);
            Abstract.Models.Installation installation = _installationService.GetInstallation(installationId)!;

            Models.HistoryDataModel model = new()
            {
                SelectedDate = DateTime.Now.Date,
                InstallationId = installationId,
                GraphType = Abstract.Models.EGraphType.Daily,
                InstallationName = installation.Name,
            };

            if (data.Any())
            {
                decimal prevIn = data.First().KwhInT1 + data.First().KwhInT2;
                decimal prevOut = data.First().KwhOutT1 + data.First().KwhOutT2;
                model.HistoryData = data.Select(d =>
                {
                    Models.HistoryDataEntry result = new()
                    {
                        Time = d.Time.ToLocalTime(),
                        KwhIn = decimal.Round(d.KwhInT1 + d.KwhInT2 - prevIn, 3),
                        KwhOut = decimal.Round(d.KwhOutT1 + d.KwhOutT2 - prevOut, 3),
                    };
                    prevIn = d.KwhInT1 + d.KwhInT2;
                    prevOut = d.KwhOutT1 + d.KwhOutT2;
                    return result;
                });
            }

            return View(model);
        }

        [HttpPost]
        [Route("Installation/{installationId}/History/{timespan}/{time}")]
        public IActionResult HistoryPartial([FromRoute] int installationId, [FromRoute] Abstract.Models.EGraphType timespan, [FromRoute] DateTime time)
        {
            if (!_currentUserService.CanAccessInstallation(installationId))
            {
                return Unauthorized();
            }

            IEnumerable<Abstract.Models.MeterData> data = _dataService.GetData(installationId, time, timespan);
            Abstract.Models.Installation installation = _installationService.GetInstallation(installationId)!;

            Models.HistoryDataModel model = new()
            {
                SelectedDate = time,
                InstallationId = installationId,
                GraphType = timespan,
                InstallationName = installation.Name,
            };

            if (data.Any())
            {
                decimal prevIn = data.First().KwhInT1 + data.First().KwhInT2;
                decimal prevOut = data.First().KwhOutT1 + data.First().KwhOutT2;
                model.HistoryData = data.Select(d =>
                {
                    Models.HistoryDataEntry result = new()
                    {
                        Time = d.Time.ToLocalTime(),
                        KwhIn = decimal.Round(d.KwhInT1 + d.KwhInT2 - prevIn, 3),
                        KwhOut = decimal.Round(d.KwhOutT1 + d.KwhOutT2 - prevOut, 3),
                    };
                    prevIn = d.KwhInT1 + d.KwhInT2;
                    prevOut = d.KwhOutT1 + d.KwhOutT2;
                    return result;
                });
            }

            return PartialView("_HistoryChartPartial", model);
        }
    }
}
