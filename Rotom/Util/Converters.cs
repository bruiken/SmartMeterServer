namespace Rotom.Util
{
    public static class Converters
    {
        public static Abstract.Models.CreateUser Convert(Models.CreateUserModel model)
        {
            return new Abstract.Models.CreateUser
            {
                Username = model.Username,
                Password = model.Password,
                RoleId = model.RoleId,
            };
        }

        public static Models.UserModel Convert(Abstract.Models.User model)
        {
            return new Models.UserModel
            {
                Id = model.Id,
                Username = model.Username,
                RoleId = model.RoleId,
                RoleName = model.RoleName,
            };
        }

        public static Models.RoleModel Convert(Abstract.Models.Role model)
        {
            return new Models.RoleModel
            {
                Id = model.Id,
                Name = model.Name,
                Permissions = Convert(model.Permissions),
            };
        }

        public static Models.PermissionsModel Convert(Abstract.Models.Permissions model)
        {
            return new Models.PermissionsModel
            {
                CanModifyUsers = model.CanModifyUsers,
                CanModifySettings = model.CanModifySettings,
                CanModifyInstallations = model.CanModifyInstallations,
            };
        }

        public static Models.SettingsModel Convert(Abstract.Models.Settings model)
        {
            return new Models.SettingsModel
            {
                RabbitMQHostname = model.RabbitMQHostname,
                RabbitMQPort = model.RabbitMQPort,
            };
        }

        public static Abstract.Models.Settings Convert(Models.SettingsModel model)
        {
            return new Abstract.Models.Settings
            {
                RabbitMQHostname = model.RabbitMQHostname,
                RabbitMQPort = model.RabbitMQPort,
            };
        }

        public static Models.InstallationModel Convert(Abstract.Models.Installation model)
        {
            return new Models.InstallationModel
            {
                Id = model.Id,
                LocationId = model.LocationId,
                Name = model.Name,
                RabbitMQUsername = model.RabbitMQUsername,
                RabbitMQPassword = model.RabbitMQPassword,
                RabbitMQExchange = model.RabbitMQExchange,
                RabbitMQVHost = model.RabbitMQVHost,
                InstallationAccesses = model.InstallationAccesses.Select(Convert),
            };
        }

        public static Models.InstallationAccessModel Convert(Abstract.Models.InstallationAccess model)
        {
            return new Models.InstallationAccessModel
            {
                UserId = model.UserId,
                Username = model.Username,
            };
        }

        public static Models.CreateInstallationModel Convert(IEnumerable<Abstract.Models.User> model)
        {
            return new Models.CreateInstallationModel
            {
                InstallationAccesses = model.Select(u => new Models.CreateInstallationAccessModel
                {
                    Selected = false,
                    UserId = u.Id,
                    Username = u.Username,
                }).ToList(),
            };
        }

        public static Models.CreateInstallationModel Convert(IEnumerable<Abstract.Models.User> users, Abstract.Models.Installation model)
        {
            IEnumerable<int> selectedUserIds = model.InstallationAccesses.Select(i => i.UserId);
            return new Models.CreateInstallationModel
            {
                Id = model.Id,
                LocationId = model.LocationId,
                Name = model.Name,
                RabbitMQUsername = model.RabbitMQUsername,
                RabbitMQPassword = model.RabbitMQPassword,
                RabbitMQExchange = model.RabbitMQExchange,
                RabbitMQVHost = model.RabbitMQVHost,
                InstallationAccesses = model.InstallationAccesses.Select(u => new Models.CreateInstallationAccessModel
                {
                    Selected = true,
                    UserId = u.UserId,
                    Username = u.Username,
                }).Union(users.Where(u => !selectedUserIds.Contains(u.Id)).Select(u => new Models.CreateInstallationAccessModel
                {
                    Selected = false,
                    UserId = u.Id,
                    Username = u.Username,
                })).ToList(),
            };
        }

        public static Abstract.Models.Installation Convert(Models.CreateInstallationModel model)
        {
            return new Abstract.Models.Installation
            {
                Id = model.Id,
                LocationId = model.LocationId,
                Name = model.Name,
                RabbitMQUsername = model.RabbitMQUsername,
                RabbitMQPassword = model.RabbitMQPassword,
                RabbitMQExchange = model.RabbitMQExchange,
                RabbitMQVHost = model.RabbitMQVHost,
                InstallationAccesses = model.InstallationAccesses
                    .Where(i => i.Selected)
                    .Select(i => new Abstract.Models.InstallationAccess
                    {
                        UserId = i.UserId,
                        Username = i.Username,
                    }),
            };
        }
    }
}
