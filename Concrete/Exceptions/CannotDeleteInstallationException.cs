namespace Concrete.Exceptions
{
    public class CannotDeleteInstallationException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = ErrorKeys.Keys.CannotDeleteInstallation;
        public override string MessageKey { get; set; } = ErrorKeys.Messages.InstallationDoesNotExist;
    }
}
