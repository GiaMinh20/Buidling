using System;

namespace API.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public int ItemPrice { get; set; }

        public int ElectricPrice { get; set; }
        public string ElectricBillUrl { get; set; }

        public int WaterPrice { get; set; }
        public string WaterBillUrl { get; set; }

        public int VehiclePrice { get; set; }
        public int OtherPrice { get; set; }
        
        public int ItemId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public bool Paied { get; set; }
        public DateTime DatePaied { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public int SumPrice()
        {
            return ItemPrice + ElectricPrice + WaterPrice + VehiclePrice + OtherPrice;
        }
    }
}
