using System.ComponentModel.DataAnnotations;

namespace Rotom.Models.DeltaAnalysis
{
    public class GasDeltaAnalysis : IDeltaAnalysis
    {
        public string Name { get; set; }

        public string Unit => "m3";
        public bool IncreaseIsPositive { get; set; } = false;

        [DisplayFormat(DataFormatString = "{0:0.###}")]
        public decimal UsageCurrentTimePeriod { get; set; }

        public decimal? UsagePreviousTimePeriod { get; set; }
    }
}
