using System;

namespace API.Payloads.Response
{
    public class PaymentResponse
    {
        public string Title { get; set; }
        public int ItemPrice { get; set; }
        public int ElectricPrice { get; set; }
        public int WaterPrice { get; set; }
        public int VehiclePrice { get; set; }
        public int OtherPrice { get; set; }
        public int TotalPrice { get; set; }
        public int ItemId { get; set; }
        public int AccountId { get; set; }
        public DateTime Time { get; set; }
    }
}
