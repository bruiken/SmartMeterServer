using Abstract.Models;

namespace Concrete.Services
{
    public class InstallationService : Abstract.Services.IInstallationService
    {
        private readonly Data.SmartMeterContext _db;

        public InstallationService(Data.SmartMeterContext db)
        {
            _db = db;
        }

        private void ValidateInstallationModel(Installation installation)
        {
            if (string.IsNullOrWhiteSpace(installation.Name)
                || _db.Installations.Any(i => i.Id != installation.Id && i.Name.ToLower() == installation.Name.ToLower()))
            {
                throw new Exceptions.InvalidInstallationException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateInstallation,
                    MessageKey = Exceptions.ErrorKeys.Messages.InvalidInstallationName,
                };
            }
            if (string.IsNullOrWhiteSpace(installation.LocationId)
                || installation.LocationId.Contains(' ')
                || _db.Installations.Any(i => i.Id != installation.Id && i.LocationId.ToLower() == installation.LocationId.ToLower()))
            {
                throw new Exceptions.InvalidInstallationException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateInstallation,
                    MessageKey = Exceptions.ErrorKeys.Messages.InvalidInstallationLocationId,
                };
            }
            if (string.IsNullOrWhiteSpace(installation.RabbitMQUsername)
                || string.IsNullOrWhiteSpace(installation.RabbitMQPassword)
                || string.IsNullOrWhiteSpace(installation.RabbitMQExchange)
                || string.IsNullOrWhiteSpace(installation.RabbitMQVHost))
            {
                throw new Exceptions.InvalidInstallationException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateInstallation,
                    MessageKey = Exceptions.ErrorKeys.Messages.InvalidInstallationRabbitMQSettings,
                };
            }
            foreach (InstallationAccess access in installation.InstallationAccesses)
            {
                if (!_db.Users.Any(u => u.Id == access.UserId))
                {
                    throw new Exceptions.InvalidInstallationException
                    {
                        ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateInstallation,
                        MessageKey = Exceptions.ErrorKeys.Messages.InvalidInstallationAccessUserId,
                    };
                }
            }
        }

        public int CreateInstallation(Installation installation)
        {
            ValidateInstallationModel(installation);

            Data.Models.Installation dbInstallation = Util.Converters.Convert(installation);
            dbInstallation.Id = 0;

            _db.Installations.Add(dbInstallation);
            _db.SaveChanges();

            foreach (InstallationAccess access in installation.InstallationAccesses)
            {
                _db.InstallationsAccesses.Add(new Data.Models.InstallationAccess
                {
                    InstallationId = dbInstallation.Id,
                    UserId = access.UserId,
                });
            }
            _db.SaveChanges();

            return dbInstallation.Id;
        }

        public void DeleteInstallation(int id)
        {
            Data.Models.Installation? dbInstallation = _db.Installations
                .SingleOrDefault(u => u.Id == id);
            if (dbInstallation == null)
            {
                throw new Exceptions.CannotDeleteInstallationException();
            }

            _db.Installations.Remove(dbInstallation);
            _db.InstallationsAccesses.RemoveRange(_db.InstallationsAccesses.Where(i => i.InstallationId == id));
            _db.SaveChanges();
        }

        public Installation? GetInstallation(int id)
        {
            Data.Models.Installation? dbInstallation = _db.Installations.SingleOrDefault(i => i.Id == id);

            if (dbInstallation == null)
            {
                return null;
            }

            Installation installation = Util.Converters.Convert(dbInstallation);

            var accesses = _db.InstallationsAccesses
                .Join(_db.Users, l => new { l.UserId, l.InstallationId }, r => new { UserId = r.Id, InstallationId = id }, (_, r) => r)
                .ToList();
            installation.InstallationAccesses = accesses.Select(a => new InstallationAccess
            {
                UserId = a.Id,
                Username = a.Username,
            });

            return installation;
        }

        public IEnumerable<Installation> GetInstallations()
        {
            var installationAccesses = _db.InstallationsAccesses
                .AsEnumerable()
                .GroupBy(k => k.InstallationId)
                .ToDictionary(k => k.Key, v => v.Select(vi => vi.UserId));

            return _db.Installations
                .ToList()
                .Select(i =>
                {
                    Installation installation = Util.Converters.Convert(i);
                    if (installationAccesses.ContainsKey(i.Id))
                    {
                        installation.InstallationAccesses = installationAccesses[i.Id].Select(v => new InstallationAccess
                        {
                            UserId = v,
                        });
                    }
                    else
                    {
                        installation.InstallationAccesses = Enumerable.Empty<InstallationAccess>();
                    }
                    return installation;
                });
        }

        public IEnumerable<Installation> GetUserInstallations(int userId)
        {
            return _db.Installations
                .Join(
                    _db.InstallationsAccesses,
                    l => new { InstallationId = l.Id, UserId = userId },
                    r => new { r.InstallationId, r.UserId },
                    (l, _) => l
                )
                .Select(Util.Converters.Convert);
        }

        public void UpdateInstallation(Installation installation)
        {
            Data.Models.Installation? dbInstallation = _db.Installations
                .SingleOrDefault(i => i.Id == installation.Id);
            if (dbInstallation == null)
            {
                throw new Exceptions.InvalidInstallationException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateInstallation,
                    MessageKey = Exceptions.ErrorKeys.Messages.InstallationDoesNotExist,
                };
            }
            ValidateInstallationModel(installation);

            dbInstallation.LocationId = installation.LocationId;
            dbInstallation.Name = installation.Name;
            dbInstallation.RabbitMQUsername = installation.RabbitMQUsername;
            dbInstallation.RabbitMQPassword = installation.RabbitMQPassword;
            dbInstallation.RabbitMQExchange = installation.RabbitMQExchange;
            dbInstallation.RabbitMQVHost = installation.RabbitMQVHost;

            IEnumerable<int> newUserIds = installation.InstallationAccesses
                .Select(i => i.UserId);
            IEnumerable<Data.Models.InstallationAccess> accesses = _db.InstallationsAccesses
                .Where(i => i.InstallationId == installation.Id);
            foreach (Data.Models.InstallationAccess access in accesses)
            {
                if (!newUserIds.Contains(access.UserId))
                {
                    _db.InstallationsAccesses.Remove(access);
                }
            }
            IEnumerable<int> accessibleUserIds = accesses.Select(a => a.UserId);
            foreach (int userId in newUserIds.Where(i => !accessibleUserIds.Contains(i)))
            {
                _db.InstallationsAccesses.Add(new Data.Models.InstallationAccess
                {
                    InstallationId = installation.Id,
                    UserId = userId,
                });
            }
            _db.SaveChanges();
        }
    }
}
