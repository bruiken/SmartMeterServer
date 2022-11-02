namespace Rotom.Models.DeltaAnalysis
{
    public interface IDeltaAnalysis
    {
        public string Name { get; set; }
        public string Unit { get; }
        public bool IncreaseIsPositive { get; }
        public decimal UsageCurrentTimePeriod { get; set; }
        public decimal? UsagePreviousTimePeriod { get; set; }

        public bool ShowIncrease => UsagePreviousTimePeriod.HasValue && UsagePreviousTimePeriod.Value != 0;
        public bool IsIncrease => UsagePreviousTimePeriod! < UsageCurrentTimePeriod;
        public bool IsPositive => (IncreaseIsPositive && IsIncrease) || (!IncreaseIsPositive && !IsIncrease);
        public int ChangePercentage => (int)(Math.Abs((UsageCurrentTimePeriod - UsagePreviousTimePeriod!.Value) / UsagePreviousTimePeriod!.Value) * 100m);
    }
}
