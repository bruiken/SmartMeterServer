namespace Abstract.Services
{
    public interface IDataService
    {
        void SaveData(int installationId, Models.MeterData data);
    }
}
