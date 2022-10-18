namespace Concrete.Exceptions
{
    public class InvalidInstallationException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = string.Empty;
        public override string MessageKey { get; set; } = string.Empty;
    }
}
