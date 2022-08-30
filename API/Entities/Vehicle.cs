using System;

namespace API.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string LicensePlates { get; set; }
        public string Transportation { get; set; }
        public bool Status { get; set; } = false;
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
