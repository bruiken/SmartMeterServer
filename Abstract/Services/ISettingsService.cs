namespace Abstract.Services
{
    public interface ISettingsService
    {
        Models.Settings GetSettings();

        void SaveSettings(Models.Settings settings);
    }
}
