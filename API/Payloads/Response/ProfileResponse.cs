using API.Entities;
using API.Helpers;
using System.Collections.Generic;

namespace API.Payloads.Response
{
    public class ProfileResponse
    {
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
