namespace Rotom.Models.HistoryData
{
    public class ElectricityDataEntry : IHistoryDataEntry
    {
        
        public long TimeX => (new DateTimeOffset(Time, TimeZone.GetUtcOffset(Time))).ToUnixTimeMilliseconds();

        public decimal ValueY => decimal.Round(KwhOut - KwhIn, 3);

        public DateTime Time { get; set; }

        public decimal KwhIn { get; set; }

        public decimal KwhOut { get; set; }

        public TimeZoneInfo TimeZone { get; set; }
    }
}
