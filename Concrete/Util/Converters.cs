namespace Concrete.Util
{
    static class Converters
    {
        public static Abstract.Models.User Convert(Data.Models.User user, Data.Models.Role role)
        {
            return new Abstract.Models.User
            {
                Id = user.Id,
                Username = user.Username,
                RoleId = user.RoleId,
                RoleName = role.Name,
            };
        }

        public static Abstract.Models.Role Convert(Data.Models.Role role, Data.Models.Permission permissions)
        {
            return new Abstract.Models.Role
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = Convert(permissions),
            };
        }

        public static Abstract.Models.Permissions Convert(Data.Models.Permission permissions)
        {
            return new Abstract.Models.Permissions
            {
                CanModifyUsers = permissions.CanModifyUsers,
                CanModifySettings = permissions.CanModifySettings,
            };
        }

        public static Abstract.Models.Settings Convert(IEnumerable<Data.Models.Setting> settings)
        {
            return new Abstract.Models.Settings
            {
                RabbitMQHostname = settings.SingleOrDefault(s => s.Key == Data.Models.SettingKey.RabbitMQHostname)?.Value ?? string.Empty,
                RabbitMQPort = settings.SingleOrDefault(s => s.Key == Data.Models.SettingKey.RabbitMQPort)?.Value ?? string.Empty,
            };
        }
    }
}
