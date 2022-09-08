using Microsoft.AspNetCore.Http;

namespace API.Payloads.Requests
{
    public class CreateMonthlyBillRequest
    {
        public int ElectricPrice { get; set; }
        public IFormFile ElectricBillUrl { get; set; }
        public int WaterPrice { get; set; }
        public IFormFile WaterBillUrl { get; set; }
        public int VehiclePrice { get; set; }
        public int OtherPrice { get; set; }
    }
}
