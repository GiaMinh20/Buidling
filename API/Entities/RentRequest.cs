using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class RentRequest
    {
        public int Id { get; set; }
        public bool Status { get; set; } = false;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required]
        public string FullName { get; set; }
        [Required]
        public string CCCD { get; set; }
        [Required]
        public int NumberOfParent { get; set; }

        public int RenterId { get; set; }
        public virtual Account Renter { get; set; }
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}
