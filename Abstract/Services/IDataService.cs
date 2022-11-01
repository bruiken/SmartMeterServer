namespace Abstract.Services
{
    public interface IDataService
    {
        int ReportingFrequencyMinutes { get; }

        void SaveData(int installationId, Models.MeterData data);

        IEnumerable<Models.MeterData> GetData(int installationId, DateTime date, Models.EGraphType graphType);
    }
}
