namespace Abstract.Models
{
    public class DeltaAnalysis
    {
        public decimal TotalKwhInCurrent { get; set; }
        public decimal TotalKwhOutCurrent { get; set; }
        public decimal TotalGasCurrent { get; set; }

        public decimal? TotalKwhInPrevious { get; set; }
        public decimal? TotalKwhOutPrevious { get; set; }
        public decimal? TotalGasPrevious { get; set; }
    }
}