namespace Abstract.Exceptions
{
    public abstract class SmartMeterException : Exception
    {
        public abstract string ErrorKey { get; set; }

        public abstract string MessageKey { get; set; }
    }
}
