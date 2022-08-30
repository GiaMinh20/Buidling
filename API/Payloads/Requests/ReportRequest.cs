using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class ReportRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        public IFormFile AvatarUrl { get; set; }
        [Required]
        public IFormFileCollection PictureUrl { get; set; }
    }
}
