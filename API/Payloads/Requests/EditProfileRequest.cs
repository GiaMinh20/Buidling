using Microsoft.AspNetCore.Http;

namespace API.Payloads.Requests
{
    public class EditProfileRequest
    {
        public IFormFile Avatar { get; set; }
        public string PhoneNumber { get; set; }
    }
}
