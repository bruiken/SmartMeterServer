namespace Concrete.Exceptions
{
    public static class ErrorKeys
    {
        public static class Keys
        {
            public const string CannotLogin = "Keys.CannotLogin";
            public const string CannotCreateUser = "Keys.CannotCreateUser";
            public const string CannotDeleteUser = "Keys.CannotDeleteUser";
            public const string CannotSaveSettings = "Keys.CannotSaveSettings";
            public const string CannotCreateInstallation = "Keys.CannotCreateInstallation";
            public const string CannotDeleteInstallation = "Keys.CannotDeleteInstallation";
            public const string CannotGenerateApiToken = "Keys.CannotGenerateApiToken";
        }

        public static class Messages
        {
            public const string InvalidUsernamePassword = "Messages.InvalidUsernamePassword";
            public const string InvalidModel = "Messages.InvalidModel";
            public const string UsernameIsTaken = "Messages.UsernameIsTaken";
            public const string CannotDeleteOwnUser = "Messages.CannotDeleteOwnUser";
            public const string UserDoesNotExist = "Messages.UserDoesNotExist";
            public const string InvalidRole = "Messages.InvalidRole";
            public const string HostnameMustNotBeEmpty = "Messages.HostnameMustNotBeEmpty";
            public const string PortMustBeValid = "Messages.PortMustBeValid";
            public const string InvalidInstallationName = "Messages.InvalidInstallationName";
            public const string InvalidInstallationLocationId = "Messages.InvalidInstallationLocationId";
            public const string InvalidInstallationRabbitMQSettings = "Messages.InvalidInstallationRabbitMQSettings";
            public const string InvalidInstallationAccessUserId = "Messages.InvalidInstallationAccessUserId";
            public const string InstallationDoesNotExist = "Messages.InstallationDoesNotExist";
            public const string InvalidTimeZone = "Messages.InvalidTimeZone";
        }
    }
}
