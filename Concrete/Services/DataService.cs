using Microsoft.EntityFrameworkCore;

namespace Concrete.Services
{
    public class DataService : Abstract.Services.IDataService
    {
        private readonly Data.SmartMeterContext _db;
        private readonly Abstract.Services.IInstallationService _installationService;

        public DataService(Data.SmartMeterContext db, Abstract.Services.IInstallationService installationService)
        {
            _db = db;
            _installationService = installationService;
        }

        public int ReportingFrequencyMinutes => 5;

        public void SaveData(int installationId, Abstract.Models.MeterData data)
        {
            Data.Models.MeterData dataModel = Util.Converters.Convert(data);
            dataModel.InstallationId = installationId;

            _db.MeterData.Add(dataModel);
            _db.SaveChanges();
        }

        private static int GetMinuteDelta(Abstract.Models.EGraphType graphType)
        {
            return graphType switch
            {
                Abstract.Models.EGraphType.Monthly => 160,
                Abstract.Models.EGraphType.Weekly => 40,
                Abstract.Models.EGraphType.Daily => 5,
                _ => throw new ArgumentException($"Unknown GraphType \"{graphType}\""),
            };
        }

        private static (DateTime start, DateTime end) GetBoundaryDates(Abstract.Models.EGraphType graphType, DateTime startTime)
        {
            switch (graphType)
            {
                case Abstract.Models.EGraphType.Monthly:
                    DateTime temp = new(startTime.Year, startTime.Month, 1);
                    return (temp, temp.AddMonths(1).AddMilliseconds(-1));
                case Abstract.Models.EGraphType.Weekly:
                    DayOfWeek weekStartDay = Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                    int diffDays = startTime.DayOfWeek - weekStartDay;
                    if (diffDays < 0)
                    {
                        diffDays += 7;
                    }
                    temp = startTime.Date.AddDays(-diffDays);
                    return (temp, temp.AddDays(7).AddMilliseconds(-1));
                case Abstract.Models.EGraphType.Daily:
                    return (startTime.Date, startTime.Date.AddDays(1).AddMilliseconds(-1));
            }
            throw new ArgumentException($"Unknown GraphType \"{graphType}\"");
        }

        public IEnumerable<Abstract.Models.MeterData> GetData(int installationId, DateTime date, Abstract.Models.EGraphType graphType)
        {
            Abstract.Models.Installation? installation = _installationService.GetInstallation(installationId);
            if (installation == null)
            {
                throw new ArgumentException("The given installation does not exist");
            }

            (DateTime start, DateTime end) = GetBoundaryDates(graphType, date);

            DateTime utcStart = start - TimeZoneInfo.FindSystemTimeZoneById(installation.Timezone).GetUtcOffset(start);
            DateTime utcEnd = end - TimeZoneInfo.FindSystemTimeZoneById(installation.Timezone).GetUtcOffset(end);

            int minuteDelta = GetMinuteDelta(graphType);
            return _db.MeterData
                .Where(d => d.InstallationId == installationId)
                .Where(d => d.Time >= utcStart)
                .Where(d => d.Time <= utcEnd)
                .Where(d => EF.Functions.DateDiffMinute(utcStart, d.Time) % minuteDelta < ReportingFrequencyMinutes)
                .ToList()
                .Select(Util.Converters.Convert);
        }

        private static DateTime GetPreviousPeriodDate(DateTime date, Abstract.Models.EGraphType graphType)
        {
            return graphType switch
            {
                Abstract.Models.EGraphType.Daily => date.AddDays(-1),
                Abstract.Models.EGraphType.Weekly => date.AddDays(-7),
                Abstract.Models.EGraphType.Monthly => date.AddMonths(-1),
                _ => throw new ArgumentException($"Unknown GraphType {graphType}"),
            };
        }

        public Abstract.Models.DeltaAnalysis? GetAnalysis(int installationId, DateTime date, Abstract.Models.EGraphType graphType)
        {
            Abstract.Models.Installation? installation = _installationService.GetInstallation(installationId);
            if (installation == null)
            {
                throw new ArgumentException("The given installation does not exist");
            }

            (DateTime curStart, DateTime curEnd) = GetBoundaryDates(graphType, date);
            (DateTime prevStart, DateTime prevEnd) = GetBoundaryDates(graphType, GetPreviousPeriodDate(date, graphType));

            IQueryable<Data.Models.MeterData> currentDataRange = _db.MeterData
                .Where(d => d.InstallationId == installationId)
                .Where(d => d.Time >= curStart)
                .Where(d => d.Time <= curEnd);
            Data.Models.MeterData? curFirstData = currentDataRange
                .OrderBy(d => d.Time)
                .FirstOrDefault();
            Data.Models.MeterData? curLastData = currentDataRange
                .OrderByDescending(d => d.Time)
                .FirstOrDefault();

            IQueryable<Data.Models.MeterData> previousDataRange = _db.MeterData
                .Where(d => d.InstallationId == installationId)
                .Where(d => d.Time >= prevStart)
                .Where(d => d.Time <= prevEnd);
            Data.Models.MeterData? prevFirstData = previousDataRange
                .OrderBy(d => d.Time)
                .FirstOrDefault();
            Data.Models.MeterData? prevLastData = previousDataRange
                .OrderByDescending(d => d.Time)
                .FirstOrDefault();

            if (!currentDataRange.Any())
            {
                return null;
            }

            return new Abstract.Models.DeltaAnalysis
            {
                TotalKwhInCurrent = (curLastData.KwhInT1 - curFirstData.KwhInT1) + (curLastData.KwhInT2 - curFirstData.KwhInT2),
                TotalKwhOutCurrent = (curLastData.KwhOutT1 - curFirstData.KwhOutT1) + (curLastData.KwhOutT2 - curFirstData.KwhOutT2),
                TotalGasCurrent = curLastData.GasReadout - curFirstData.GasReadout,
                TotalKwhInPrevious = previousDataRange.Any() ? (prevLastData.KwhInT1 - prevFirstData.KwhInT1) + (prevLastData.KwhInT2 - prevFirstData.KwhInT2) : null,
                TotalKwhOutPrevious = previousDataRange.Any() ? (prevLastData.KwhOutT1 - prevFirstData.KwhOutT1) + (prevLastData.KwhOutT2 - prevFirstData.KwhOutT2) : null,
                TotalGasPrevious = previousDataRange.Any() ? prevLastData.GasReadout - prevFirstData.GasReadout : null,
            };
        }
    }
}
