using API.Entities;
using API.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class CreateRentRequest
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string CCCD { get; set; }
        [Required]
        public int NumberOfParent { get; set; }
        public int ItemId { get; set; }
    }
}
