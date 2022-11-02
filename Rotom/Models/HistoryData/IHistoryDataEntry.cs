using System.Text.Json.Serialization;

namespace Rotom.Models.HistoryData
{
    public interface IHistoryDataEntry
    {
        [JsonInclude]
        [JsonPropertyName("x")]
        [JsonPropertyOrder(1)]
        public long TimeX { get; }

        [JsonInclude]
        [JsonPropertyName("y")]
        [JsonPropertyOrder(2)]
        public decimal ValueY { get; }

        [JsonIgnore]
        public TimeZoneInfo TimeZone { get; set; }
    }
}
