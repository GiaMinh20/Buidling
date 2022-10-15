using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Payloads.Requests
{
    public class EditItemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public EItemStatus Status { get; set; }
        public IFormFile AvatarUrl { get; set; }
        public IFormFileCollection PictureUrl { get; set; }
        public IFormFile VideoUrl { get; set; }
    }
}
