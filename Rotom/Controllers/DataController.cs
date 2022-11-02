using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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
        private readonly IStringLocalizer<DataController> _localizer;

        public DataController(
            Abstract.Services.IInstallationService installationService,
            Abstract.Services.ICurrentUserService currentUserService,
            Abstract.Services.ISettingsService settingsService,
            Abstract.Services.IDataService dataService,
            IStringLocalizer<DataController> localizer)
        {
            _installationService = installationService;
            _currentUserService = currentUserService;
            _settingsService = settingsService;
            _dataService = dataService;
            _localizer = localizer;
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

        private IEnumerable<Models.DeltaAnalysis.IDeltaAnalysis> CreateAnalysisModels(int installationId, DateTime date, Abstract.Models.EGraphType graphType)
        {
            Abstract.Models.DeltaAnalysis? analysis = _dataService.GetAnalysis(installationId, date, graphType);

            if (analysis != null)
            {
                return new[]
                {
                    new Models.DeltaAnalysis.ElectricityDeltaAnalysis
                    {
                        Name = _localizer[Resources.Controllers.DataController.KwhInChange],
                        IncreaseIsPositive = false,
                        UsageCurrentTimePeriod = analysis.TotalKwhInCurrent,
                        UsagePreviousTimePeriod = analysis.TotalKwhInPrevious,
                    },
                    new Models.DeltaAnalysis.ElectricityDeltaAnalysis
                    {
                        Name = _localizer[Resources.Controllers.DataController.KwhOutChange],
                        IncreaseIsPositive = true,
                        UsageCurrentTimePeriod = analysis.TotalKwhOutCurrent,
                        UsagePreviousTimePeriod = analysis.TotalKwhOutPrevious,
                    },
                };
            }
            else
            {
                return Enumerable.Empty<Models.DeltaAnalysis.IDeltaAnalysis>();
            }
        }

        private Models.HistoryDataModel CreateModel(int installationId, DateTime date, Abstract.Models.EGraphType graphType)
        {
            IEnumerable<Abstract.Models.MeterData> data = _dataService.GetData(installationId, date, graphType);
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
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(installation.Timezone);
                model.HistoryData = data.Select(d =>
                {
                    Models.HistoryDataEntry result = new()
                    {
                        Time = TimeZoneInfo.ConvertTimeFromUtc(d.Time, tzi),
                        KwhIn = decimal.Round(d.KwhInT1 + d.KwhInT2 - prevIn, 3),
                        KwhOut = decimal.Round(d.KwhOutT1 + d.KwhOutT2 - prevOut, 3),
                    };
                    prevIn = d.KwhInT1 + d.KwhInT2;
                    prevOut = d.KwhOutT1 + d.KwhOutT2;
                    return result;
                });
            }

            model.DeltaAnalysis = CreateAnalysisModels(installationId, date, graphType);

            return model;
        }

        [HttpGet]
        [Route("Installation/{installationId}/History")]
        public IActionResult History([FromRoute] int installationId)
        {
            if (!_currentUserService.CanAccessInstallation(installationId))
            {
                return Unauthorized();
            }

            Models.HistoryDataModel model = CreateModel(installationId, DateTime.Today, Abstract.Models.EGraphType.Daily);
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

            Models.HistoryDataModel model = CreateModel(installationId, time, timespan);
            return PartialView("_HistoryChartPartial", model);
        }
    }
}
