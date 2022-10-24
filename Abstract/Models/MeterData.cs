namespace Abstract.Models
{
    public class MeterData
    {
        public DateTime Time { get; set; }

        public decimal KwhInT1 { get; set; }
        public decimal KwhInT2 { get; set; }

        public decimal KwhOutT1 { get; set; }
        public decimal KwhOutT2 { get; set; }

        public decimal GasReadout { get; set; }
    }
}