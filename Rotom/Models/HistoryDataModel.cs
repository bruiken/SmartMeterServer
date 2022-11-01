using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rotom.Models
{
    public class HistoryDataModel
    {
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; }

        public Abstract.Models.EGraphType GraphType { get; set; }

        public int InstallationId { get; set; }

        public string InstallationName { get; set; }

        public IEnumerable<HistoryDataEntry> HistoryData { get; set; }

        public string HistoryJson => System.Text.Json.JsonSerializer.Serialize(HistoryData);
    }

    public class HistoryDataEntry
    {
        [JsonInclude]
        [JsonPropertyName("x")]
        [JsonPropertyOrder(1)]
        public long TimeX => (new DateTimeOffset(Time)).ToUnixTimeMilliseconds();

        [JsonInclude]
        [JsonPropertyName("y")]
        [JsonPropertyOrder(2)]
        public decimal KwhDeltaY => decimal.Round(KwhOut - KwhIn, 3);

        [JsonIgnore]
        public DateTime Time { get; set; }

        [JsonIgnore]
        public decimal KwhIn { get; set; }

        [JsonIgnore]
        public decimal KwhOut { get; set; }
    }
}
