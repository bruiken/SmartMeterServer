namespace Concrete.Services
{
    public class DataService : Abstract.Services.IDataService
    {
        private readonly Data.SmartMeterContext _db;

        public DataService(Data.SmartMeterContext db)
        {
            _db = db;
        }

        public void SaveData(int installationId, Abstract.Models.MeterData data)
        {
            Data.Models.MeterData dataModel = Util.Converters.Convert(data);
            dataModel.InstallationId = installationId;

            _db.MeterData.Add(dataModel);
            _db.SaveChanges();
        }
    }
}
