using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class CreateUnRentRequest
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string CCCD { get; set; }
        [Required]
        public int ItemId { get; set; }
    }
}
