namespace API.Payloads.Response
{
    public class MonthlyRevenueResponse
    {
        public double TotalRentPrice { get; set; } = 0;
        public double TotalWaterPrice { get; set; } = 0;
        public double TotalElectricPrice { get; set; } = 0;
        public double TotalPaidPrice { get; set; } = 0;
        public double TotalStriprFee { get; set; } = 0;
        public double Revenue { get; set; } = 0;
    }
}
