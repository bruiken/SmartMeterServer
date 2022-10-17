namespace Concrete.Exceptions
{
    public static class ErrorKeys
    {
        public static class Keys
        {
            public const string CannotLogin = "Keys.CannotLogin";
            public const string CannotCreateUser = "Keys.CannotCreateUser";
            public const string CannotDeleteUser = "Keys.CannotDeleteUser";
        }

        public static class Messages
        {
            public const string InvalidUsernamePassword = "Messages.InvalidUsernamePassword";
            public const string InvalidModel = "Messages.InvalidModel";
            public const string UsernameIsTaken = "Messages.UsernameIsTaken";
            public const string CannotDeleteOwnUser = "Messages.CannotDeleteOwnUser";
            public const string UserDoesNotExist = "Messages.UserDoesNotExist";
            public const string InvalidRole = "Messages.InvalidRole";
        }
    }
}
