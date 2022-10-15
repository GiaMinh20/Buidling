using System;

namespace API.Payloads.Requests
{
    public class PdfBillRequest
    {
        public DateTime CreateDate { get; set; }
        public int BillId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int ItemPrice { get; set; }
        public int ElectricPrice { get; set; }
        public int WaterPrice { get; set; }
        public int VehiclePrice { get; set; }
        public int OrtherPrice { get; set; }
        public string ElectricBillUrl { get; set; }
        public string WaterBillUrl { get; set; }

        public int SumPrice()
        {
            return ItemPrice + ElectricPrice + WaterPrice + VehiclePrice + OrtherPrice;
        }
    }
}
