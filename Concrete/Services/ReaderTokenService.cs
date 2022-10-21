namespace Concrete.Services
{
    public class ReaderTokenService : Abstract.Services.IReaderTokenService
    {
        private readonly Data.SmartMeterContext _db;
        private readonly Abstract.Services.ISecurityService _securityService;
        private readonly Abstract.Services.IInstallationService _installationService;

        public ReaderTokenService(Data.SmartMeterContext db, Abstract.Services.ISecurityService securityService, Abstract.Services.IInstallationService installationService)
        {
            _db = db;
            _securityService = securityService;
            _installationService = installationService;
        }

        public string GenerateToken(int installationId)
        {
            Abstract.Models.Installation? installation = _installationService.GetInstallation(installationId);

            if (installation == null)
            {
                throw new Exceptions.InvalidInstallationException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotGenerateApiToken,
                    MessageKey = Exceptions.ErrorKeys.Messages.InstallationDoesNotExist,
                };
            }

            string token = _securityService.GenerateJwtToken(installation);

            Data.Models.ReaderApiToken? currentDbToken = _db.ReaderApiTokens
                .SingleOrDefault(r => r.InstallationId == installationId);
            if (currentDbToken != null)
            {
                _db.ReaderApiTokens.Remove(currentDbToken);
                _db.SaveChanges();
            }

            _db.ReaderApiTokens.Add(new Data.Models.ReaderApiToken
            {
                InstallationId = installationId,
                Token = token,
            });
            _db.SaveChanges();

            return token;
        }

        public int? GetInstallationIdForToken(string token)
        {
            Data.Models.ReaderApiToken? dbToken = _db.ReaderApiTokens
                .SingleOrDefault(r => r.Token == token);

            if (dbToken == null)
            {
                return null;
            }

            return dbToken.InstallationId;
        }
    }
}
