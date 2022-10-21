namespace Abstract.Services
{
    public interface IReaderTokenService
    {
        int? GetInstallationIdForToken(string token);

        string GenerateToken(int installationId);
    }
}
