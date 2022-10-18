namespace Concrete.Exceptions
{
    public class InvalidSettingsException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = ErrorKeys.Keys.CannotSaveSettings;
        public override string MessageKey { get; set; } = string.Empty;
    }
}
