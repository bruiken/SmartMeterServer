using Abstract.Models;

namespace Concrete.Services
{
    public class SettingsService : Abstract.Services.ISettingsService
    {
        private readonly Data.SmartMeterContext _db;

        public SettingsService(Data.SmartMeterContext db)
        {
            _db = db;
        }

        public Settings GetSettings()
        {
            return Util.Converters.Convert(_db.Settings);
        }

        private static void ValidateSettings(Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.RabbitMQHostname))
            {
                throw new Exceptions.InvalidSettingsException
                {
                    MessageKey = Exceptions.ErrorKeys.Messages.HostnameMustNotBeEmpty,
                };
            }
            if (string.IsNullOrWhiteSpace(settings.RabbitMQPort) || !int.TryParse(settings.RabbitMQPort, out int port) || port <= 0 || port > 65535)
            {
                throw new Exceptions.InvalidSettingsException
                {
                    MessageKey = Exceptions.ErrorKeys.Messages.PortMustBeValid,
                };
            }
        }

        private Data.Models.Setting GetOrCreateSetting(IEnumerable<Data.Models.Setting> dbSettings, Data.Models.SettingKey key)
        {
            Data.Models.Setting? setting = dbSettings.SingleOrDefault(s => s.Key == key);
            if (setting == null)
            {
                setting = new()
                {
                    Key = key,
                };
                _db.Settings.Add(setting);
            }
            return setting;
        }

        public void SaveSettings(Settings settings)
        {
            ValidateSettings(settings);

            IEnumerable<Data.Models.Setting> dbSettings = _db.Settings.ToList();

            Data.Models.Setting? setting = GetOrCreateSetting(dbSettings, Data.Models.SettingKey.RabbitMQHostname);
            setting.Value = settings.RabbitMQHostname;

            setting = GetOrCreateSetting(dbSettings, Data.Models.SettingKey.RabbitMQPort);
            setting.Value = settings.RabbitMQPort;

            _db.SaveChanges();
        }
    }
}
