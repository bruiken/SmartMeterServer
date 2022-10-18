using Abstract.Models;
using System.Reflection.Metadata.Ecma335;

namespace Concrete.Services
{
    public class RoleService : Abstract.Services.IRoleService
    {
        private readonly Data.SmartMeterContext _db;

        public RoleService(Data.SmartMeterContext db)
        {
            _db = db;
        }

        public Role? GetById(int id)
        {
            Data.Models.Role? dbRole = _db.Roles
                .SingleOrDefault(r => r.Id == id);
            if (dbRole == null)
            {
                return null;
            }

            Data.Models.Permission? dbPermissions = _db.Permissions
                .SingleOrDefault(p => p.RoleId == id);
            if (dbPermissions == null)
            {
                return null;
            }

            return Util.Converters.Convert(dbRole, dbPermissions);
        }

        public Role? GetByUser(int userId)
        {
            Data.Models.User? dbUser = _db.Users.SingleOrDefault(u => u.Id == userId);
            if (dbUser == null)
            {
                return null;
            }

            return GetById(dbUser.RoleId);
        }

        public IEnumerable<Role> GetRoles()
        {
            var dbRoles = _db.Roles.ToList();
            var dbPermissions = _db.Permissions.ToList();

            return dbRoles
                .Join(
                    dbPermissions,
                    l => l.Id,
                    r => r.RoleId,
                    (role, perms) => Util.Converters.Convert(role, perms)
                );
        }

        public bool RoleIsAllowedPermission(Role role, EPermission permission)
        {
            return permission switch
            {
                EPermission.CanModifyUsers => role.Permissions.CanModifyUsers,
                EPermission.CanModifySettings => role.Permissions.CanModifySettings,
                EPermission.CanModifyInstallations => role.Permissions.CanModifyInstallations,
                _ => false,
            };
        }

        public int SeedRoles()
        {
            if (!_db.Roles.Any())
            {
                Data.Models.Role adminRole = new()
                {
                    Name = "admin",
                };
                _db.Roles.Add(adminRole);

                Data.Models.Role userRole = new()
                {
                    Name = "user",
                };
                _db.Roles.Add(userRole);
                _db.SaveChanges();

                _db.Permissions.Add(new Data.Models.Permission
                {
                    RoleId = adminRole.Id,
                    CanModifyUsers = true,
                    CanModifySettings = true,
                    CanModifyInstallations = true,
                });
                _db.Permissions.Add(new Data.Models.Permission
                {
                    RoleId = userRole.Id,
                    CanModifyUsers = false,
                    CanModifySettings = false,
                    CanModifyInstallations = false,
                });
                _db.SaveChanges();

                return adminRole.Id;
            }
            return 1;
        }
    }
}
