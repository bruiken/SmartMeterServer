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

        private IEnumerable<Models.DeltaAnalysis.ElectricityDeltaAnalysis> GetElectricityDeltaAnalysis(Abstract.Models.DeltaAnalysis analysis)
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

        private IEnumerable<Models.DeltaAnalysis.GasDeltaAnalysis> GetGasDeltaAnalysis(Abstract.Models.DeltaAnalysis analysis)
        {
            return new[]
            {
                new Models.DeltaAnalysis.GasDeltaAnalysis
                {
                    Name = _localizer[Resources.Controllers.DataController.GasChange],
                    UsageCurrentTimePeriod = analysis.TotalGasCurrent,
                    UsagePreviousTimePeriod = analysis.TotalGasPrevious,
                },
            };
        }

        private IEnumerable<Models.DeltaAnalysis.IDeltaAnalysis> CreateAnalysisModels(int installationId, DateTime date, Abstract.Models.EGraphType graphType, Abstract.Models.EDataType dataType)
        {
            Abstract.Models.DeltaAnalysis? analysis = _dataService.GetAnalysis(installationId, date, graphType);

            if (analysis != null)
            {
                return dataType switch
                {
                    Abstract.Models.EDataType.Electricity => GetElectricityDeltaAnalysis(analysis),
                    Abstract.Models.EDataType.Gas => GetGasDeltaAnalysis(analysis),
                    _ => throw new ArgumentException($"Unknown DataType {dataType}"),
                };
            }
            else
            {
                return Enumerable.Empty<Models.DeltaAnalysis.IDeltaAnalysis>();
            }
        }

        private static IEnumerable<Models.HistoryData.ElectricityDataEntry> CreateElectricityDataEntryModels(IEnumerable<Abstract.Models.MeterData> data, TimeZoneInfo tzi)
        {
            decimal prevIn = data.First().KwhInT1 + data.First().KwhInT2;
            decimal prevOut = data.First().KwhOutT1 + data.First().KwhOutT2;
            return data.Select(d =>
            {
                Models.HistoryData.ElectricityDataEntry result = new()
                {
                    Time = d.Time,
                    KwhIn = decimal.Round(d.KwhInT1 + d.KwhInT2 - prevIn, 3),
                    KwhOut = decimal.Round(d.KwhOutT1 + d.KwhOutT2 - prevOut, 3),
                    TimeZone = tzi,
                };
                prevIn = d.KwhInT1 + d.KwhInT2;
                prevOut = d.KwhOutT1 + d.KwhOutT2;
                return result;
            });
        }

        private static IEnumerable<Models.HistoryData.GasDataEntry> CreateGasDataEntryModels(IEnumerable<Abstract.Models.MeterData> data, TimeZoneInfo tzi)
        {
            decimal prevGas = data.First().GasReadout;
            return data.Select(d =>
            {
                Models.HistoryData.GasDataEntry result = new()
                {
                    Time = d.Time,
                    GasAmount = d.GasReadout - prevGas,
                    TimeZone = tzi,
                };
                prevGas = d.GasReadout;
                return result;
            });
        }

        private Models.HistoryDataModel CreateModel(int installationId, DateTime date, Abstract.Models.EGraphType graphType, Abstract.Models.EDataType dataType)
        {
            IEnumerable<Abstract.Models.MeterData> data = _dataService.GetData(installationId, date, graphType);
            Abstract.Models.Installation installation = _installationService.GetInstallation(installationId)!;

            Models.HistoryDataModel model = new()
            {
                SelectedDate = date,
                InstallationId = installationId,
                GraphType = graphType,
                InstallationName = installation.Name,
                TypeOfData = dataType,
            };

            if (data.Any())
            {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(installation.Timezone);

                model.HistoryData = dataType switch
                {
                    Abstract.Models.EDataType.Electricity => CreateElectricityDataEntryModels(data, tzi),
                    Abstract.Models.EDataType.Gas => CreateGasDataEntryModels(data, tzi),
                    _ => throw new ArgumentException($"Unknown DataType {dataType}"),
                };
            }

            model.DeltaAnalysis = CreateAnalysisModels(installationId, date, graphType, dataType);

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

            Models.HistoryDataModel model = CreateModel(installationId, DateTime.Today, Abstract.Models.EGraphType.Daily, Abstract.Models.EDataType.Electricity);
            return View(model);
        }

        [HttpPost]
        [Route("Installation/{installationId}/History/{dataType}/{timespan}/{time}")]
        public IActionResult HistoryPartial([FromRoute] int installationId, [FromRoute] Abstract.Models.EDataType dataType, [FromRoute] Abstract.Models.EGraphType timespan, [FromRoute] DateTime time)
        {
            if (!_currentUserService.CanAccessInstallation(installationId))
            {
                return Unauthorized();
            }

            Models.HistoryDataModel model = CreateModel(installationId, time, timespan, dataType);
            return PartialView("_HistoryChartPartial", model);
        }
    }
}
