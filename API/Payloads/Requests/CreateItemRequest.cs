using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class CreateItemRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public IFormFile AvatarUrl { get; set; }
        [Required]
        public IFormFileCollection PictureUrl { get; set; }
        [Required]
        public IFormFile VideoUrl { get; set; }
        [Required]
        public int TypeId { get; set; }
    }
}
