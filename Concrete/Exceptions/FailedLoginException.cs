namespace Concrete.Exceptions
{
    public class FailedLoginException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = ErrorKeys.Keys.CannotLogin;
        public override string MessageKey { get; set; } = ErrorKeys.Messages.InvalidUsernamePassword;
    }
}
