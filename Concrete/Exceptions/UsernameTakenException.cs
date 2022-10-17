namespace Concrete.Exceptions
{
    public class UsernameTakenException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = ErrorKeys.Keys.CannotCreateUser;
        public override string MessageKey { get; set; } = ErrorKeys.Messages.UsernameIsTaken;
    }
}
