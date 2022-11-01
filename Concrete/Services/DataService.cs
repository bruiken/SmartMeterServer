using Microsoft.EntityFrameworkCore;
using System;

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
                    int diffDays = DayOfWeek.Monday - startTime.DayOfWeek;
                    temp = startTime.Date.AddDays(diffDays);
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
                .AsEnumerable()
                .Select(Util.Converters.Convert);
        }
    }
}
