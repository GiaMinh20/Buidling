using API.Entities;
using Microsoft.AspNetCore.Http;
using System;

namespace API.Payloads.Requests
{
    public class CreateMemberRequest
    {
        public string FullName { get; set; }
        public IFormFile AvatarUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string CCCD { get; set; }
        public string Nationality { get; set; }
        public UserAddress PlaceOfOrigin { get; set; }
    }
}
