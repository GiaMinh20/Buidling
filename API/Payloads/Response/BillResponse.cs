using System;

namespace API.Payloads.Response
{
    public class BillResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public int ItemPrice { get; set; }

        public int ElectricPrice { get; set; }
        public string ElectricBillUrl { get; set; }

        public int WaterPrice { get; set; }
        public string WaterBillUrl { get; set; }

        public int VehiclePrice { get; set; }
        public int OtherPrice { get; set; }

        public int SumPrice { get; set; }
        public bool Paied { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DatePaied { get; set; }
    }
}
