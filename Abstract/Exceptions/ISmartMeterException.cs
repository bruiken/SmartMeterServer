namespace Abstract.Exceptions
{
    public abstract class SmartMeterException : Exception
    {
        public abstract string ErrorKey { get; }

        public abstract string MessageKey { get; }
    }
}
