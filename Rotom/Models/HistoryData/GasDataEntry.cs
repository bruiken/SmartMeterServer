namespace Rotom.Models.HistoryData
{
    public class GasDataEntry : IHistoryDataEntry
    {
        public long TimeX => (new DateTimeOffset(Time)).ToUnixTimeMilliseconds();

        public decimal ValueY => decimal.Round(GasAmount, 3);

        public DateTime Time { get; set; }

        public decimal GasAmount { get; set; }
    }
}
