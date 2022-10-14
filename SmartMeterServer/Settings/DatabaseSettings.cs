namespace SmartMeterServer.Settings
{
    public class DatabaseSettings
    {
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int BuildVersion { get; set; }
        public string? Database { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
    }
}
