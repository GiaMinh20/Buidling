using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class UnRentRequest
    {
        public int Id { get; set; }
        public int RenterId { get; set; }
        public int ItemId { get; set; }
        public bool status { get; set; } = false;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime HandleTime { get; set; }
        public int HandlerId { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string CCCD { get; set; }
    }
}
