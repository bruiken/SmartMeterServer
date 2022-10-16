namespace Concrete.Exceptions
{
    public class FailedLoginException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey => ErrorKeys.Keys.CannotLogin;
        public override string MessageKey => ErrorKeys.Messages.InvalidUsernamePassword;
    }
}
