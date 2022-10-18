namespace Abstract.Services
{
    public interface IInstallationService
    {
        IEnumerable<Models.Installation> GetInstallations();

        IEnumerable<Models.Installation> GetUserInstallations(int userId);

        Models.Installation? GetInstallation(int id);

        int CreateInstallation(Models.Installation installation);

        void UpdateInstallation(Models.Installation installation);

        void DeleteInstallation(int id);
    }
}
