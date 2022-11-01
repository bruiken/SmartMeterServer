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
                CanModifyInstallations = permissions.CanModifyInstallations,
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

        public static Data.Models.Installation Convert(Abstract.Models.Installation installation)
        {
            return new Data.Models.Installation
            {
                Id = installation.Id,
                LocationId = installation.LocationId,
                Name = installation.Name,
                RabbitMQUsername = installation.RabbitMQUsername,
                RabbitMQPassword = installation.RabbitMQPassword,
                RabbitMQExchange = installation.RabbitMQExchange,
                RabbitMQVHost = installation.RabbitMQVHost,
            };
        }

        public static Abstract.Models.Installation Convert(Data.Models.Installation installation)
        {
            return new Abstract.Models.Installation
            {
                Id = installation.Id,
                LocationId = installation.LocationId,
                Name = installation.Name,
                RabbitMQUsername = installation.RabbitMQUsername,
                RabbitMQPassword = installation.RabbitMQPassword,
                RabbitMQExchange = installation.RabbitMQExchange,
                RabbitMQVHost = installation.RabbitMQVHost,
            };
        }

        public static Data.Models.MeterData Convert(Abstract.Models.MeterData data)
        {
            return new Data.Models.MeterData
            {
                Time = data.Time,
                KwhInT1 = data.KwhInT1,
                KwhInT2 = data.KwhInT2,
                KwhOutT1 = data.KwhOutT1,
                KwhOutT2 = data.KwhOutT2,
                GasReadout = data.GasReadout,
            };
        }

        public static Abstract.Models.MeterData Convert(Data.Models.MeterData data)
        {
            return new Abstract.Models.MeterData
            {
                Time = data.Time,
                KwhInT1 = data.KwhInT1,
                KwhInT2 = data.KwhInT2,
                KwhOutT1 = data.KwhOutT1,
                KwhOutT2 = data.KwhOutT2,
                GasReadout = data.GasReadout,
            };
        }
    }
}
