namespace Concrete.Exceptions
{
    internal class InvalidRoleException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = string.Empty;
        public override string MessageKey { get; set; } = ErrorKeys.Messages.InvalidRole;
    }
}
