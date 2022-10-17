using Abstract.Models;
using Microsoft.Extensions.Options;

namespace Concrete.Services
{
    public class UserService : Abstract.Services.IUserService
    {
        private readonly Data.SmartMeterContext _db;
        private readonly Abstract.Services.ICurrentUserService _currentUserService;
        private readonly Abstract.Services.ISecurityService _securityService;
        private readonly Abstract.Settings.CookieSettings _cookieSettings;
        private readonly Abstract.Settings.GeneralSettings _generalSettings;

        public UserService(
            Data.SmartMeterContext db, 
            Abstract.Services.ICurrentUserService currentUserService, 
            Abstract.Services.ISecurityService securityService, 
            IOptions<Abstract.Settings.CookieSettings> cookieOptions,
            IOptions<Abstract.Settings.GeneralSettings> generalOptions)
        {
            _db = db;
            _currentUserService = currentUserService;
            _securityService = securityService;
            _cookieSettings = cookieOptions.Value;
            _generalSettings = generalOptions.Value;
        }

        private void RemoveExpiredTokens(int userId)
        {
            var expired = _db.RefreshTokens
                .Where(r => r.UserId == userId)
                .Where(r => r.ExpiryDate < DateTime.UtcNow);
            _db.RefreshTokens.RemoveRange(expired);
            _db.SaveChanges();
        }

        public User? GetById(int id)
        {
            Data.Models.User? user = _db.Users
                .SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                return null;
            }
            else
            {
                Data.Models.Role? role = _db.Roles
                    .SingleOrDefault(r => r.Id == user.RoleId);

                if (role == null)
                {
                    return null;
                }

                return Util.Converters.Convert(user, role);
            }
        }

        private TimeSpan RefreshTokenValidityTimeSpan()
        {
            DateTime now = DateTime.UtcNow;

            return now.AddMonths(_cookieSettings.RefreshTokenValidityMonths).Subtract(now);
        }

        public void Login(string username, string password, bool rememberLogin)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exceptions.InvalidModelException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotLogin
                };
            }

            Data.Models.User? user = _db.Users
                .SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                throw new Exceptions.FailedLoginException();
            }

            Data.Models.Role? role = _db.Roles
                .SingleOrDefault(r => r.Id == user.RoleId);

            Hash hash = new()
            {
                Salt = user.PasswordSalt,
                Key = user.PasswordHash,
            };

            if (_securityService.Verify(hash, password))
            {
                User userModel = Util.Converters.Convert(user, role!);
                _currentUserService.SaveAccessToken(_securityService.GenerateJwtToken(userModel, TimeSpan.FromHours(_cookieSettings.AccessTokenValidityHours)));

                if (rememberLogin)
                {
                    TimeSpan refreshTokenValidityTimespan = RefreshTokenValidityTimeSpan();
                    string refreshToken = _securityService.GenerateJwtToken(userModel, refreshTokenValidityTimespan);
                    _currentUserService.SaveRefreshToken(refreshToken);
                    _db.RefreshTokens.Add(new Data.Models.RefreshToken
                    {
                        ExpiryDate = DateTime.UtcNow.Add(refreshTokenValidityTimespan),
                        Token = refreshToken,
                        UserId = user.Id,
                    });
                    _db.SaveChanges();
                    RemoveExpiredTokens(user.Id);
                }
            }
            else
            {
                throw new Exceptions.FailedLoginException();
            }
        }

        public bool TryLoginWithRefreshToken()
        {
            string? cookieRefreshToken = _currentUserService.GetRefreshTokenCookie();

            if (!string.IsNullOrEmpty(cookieRefreshToken))
            {
                var jwtToken = _securityService.ValidateToken(cookieRefreshToken, validateLifetime: false);
                if (jwtToken != null)
                {
                    Data.Models.RefreshToken? refreshToken = _db.RefreshTokens
                        .SingleOrDefault(r => r.Token == cookieRefreshToken);
                    int userId = int.Parse(jwtToken.Claims.First(c => c.Type == _securityService.IdClaim).Value);
                    User? user = GetById(userId);

                    if (refreshToken != null && user != null)
                    {
                        if (refreshToken.ExpiryDate < DateTime.UtcNow)
                        {
                            _db.RefreshTokens.Remove(refreshToken);
                            _db.SaveChanges();
                        }
                        else if (refreshToken.UserId == userId)
                        {
                            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(_cookieSettings.RefreshTokenValidityMonths);
                            _db.SaveChanges();

                            _currentUserService.SaveAccessToken(_securityService.GenerateJwtToken(user, TimeSpan.FromHours(_cookieSettings.AccessTokenValidityHours)));
                            _currentUserService.SaveRefreshToken(refreshToken.Token);

                            RemoveExpiredTokens(user.Id);
                            return true;
                        }
                    }
                }
            }

            _currentUserService.ClearTokenCookies();
            return false;
        }

        public int CreateUser(CreateUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                throw new Exceptions.InvalidModelException()
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateUser,
                };
            }
            if (_db.Users.Any(u => u.Username.ToLower() == user.Username.ToLower()))
            {
                throw new Exceptions.UsernameTakenException();
            }
            if (!_db.Roles.Any(r => r.Id == user.RoleId))
            {
                throw new Exceptions.InvalidRoleException
                {
                    ErrorKey = Exceptions.ErrorKeys.Keys.CannotCreateUser,
                };
            }

            Hash hash = _securityService.Hash(user.Password);
            Data.Models.User newUser = new()
            {
                Username = user.Username,
                RoleId = user.RoleId,
                PasswordHash = hash.Key,
                PasswordSalt = hash.Salt,
            };
            _db.Users.Add(newUser);
            _db.SaveChanges();

            return newUser.Id;
        }

        public void CreateAdminUser(int adminRoleId)
        {
            if (!_db.Users.Any())
            {
                CreateUser(new CreateUser
                {
                    Username = _generalSettings.AdminUsername,
                    Password = _generalSettings.AdminPassword,
                    RoleId = adminRoleId,
                });
            }
        }

        public void Logout()
        {
            if (_currentUserService.IsLoggedIn)
            {
                _currentUserService.ClearTokenCookies();
            }
        }

        public IEnumerable<User> GetUsers()
        {
            List<Data.Models.User> dbUsers = _db.Users.ToList();
            List<Data.Models.Role> dbRoles = _db.Roles.ToList();

            return dbUsers
                .Join(
                    dbRoles,
                    l => l.RoleId,
                    r => r.Id,
                    (user, role) => Util.Converters.Convert(user, role)
                );
        }

        public void DeleteUser(int id)
        {
            int currentUserId = _currentUserService.GetCurrentUser()!.Id;
            if (currentUserId == id)
            {
                throw new Exceptions.CannotDeleteUserException
                {
                    MessageKey = Exceptions.ErrorKeys.Messages.CannotDeleteOwnUser,
                };
            }

            Data.Models.User? dbUser = _db.Users.SingleOrDefault(u => u.Id == id);
            if (dbUser == null)
            {
                throw new Exceptions.CannotDeleteUserException
                {
                    MessageKey = Exceptions.ErrorKeys.Messages.UserDoesNotExist,
                };
            }

            _db.Users.Remove(dbUser);
            _db.RefreshTokens.RemoveRange(_db.RefreshTokens.Where(r => r.UserId == id));
            _db.SaveChanges();
        }
    }
}
