using System.ComponentModel.DataAnnotations;

namespace Rotom.Models
{
    public class HistoryDataModel
    {
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; }

        public EGraphType GraphType { get; set; }

        public EDataType TypeOfData { get; set; }

        public ECollectionType CollectionType { get; set; }

        public int InstallationId { get; set; }

        public string InstallationName { get; set; }

        public IEnumerable<HistoryData.IHistoryDataEntry> HistoryData { get; set; }

        public IEnumerable<DeltaAnalysis.IDeltaAnalysis> DeltaAnalysis { get; set; }

        public string HistoryJson => System.Text.Json.JsonSerializer.Serialize(HistoryData);
    }
}
