using System.ComponentModel.DataAnnotations;

namespace Rotom.Models.DeltaAnalysis
{
    public class ElectricityDeltaAnalysis : IDeltaAnalysis
    {
        public string Name { get; set; }

        public string Unit => "kWh";
        public bool IncreaseIsPositive { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.###}")]
        public decimal UsageCurrentTimePeriod { get; set; }
        
        public decimal? UsagePreviousTimePeriod { get; set; }
    }
}
