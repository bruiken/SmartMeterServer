using System.Runtime.Serialization;

namespace Concrete.Exceptions
{
    internal class CannotDeleteUserException : Abstract.Exceptions.SmartMeterException
    {
        public override string ErrorKey { get; set; } = ErrorKeys.Keys.CannotDeleteUser;
        public override string MessageKey { get; set; } = ErrorKeys.Messages.InvalidModel;
    }
}