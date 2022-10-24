namespace Rotom.Models
{
    public class ReportDataApiModel
    {
        public decimal KwhInT1 { get; set; }
        public decimal KwhInT2 { get; set; }

        public decimal KwhOutT1 { get; set; }
        public decimal KwhOutT2 { get; set; }

        public decimal GasReadout { get; set; }
        public DateTime Time { get; set; }
    }
}