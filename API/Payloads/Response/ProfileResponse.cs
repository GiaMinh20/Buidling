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
        public int CountPerson { get; set; }
        public List<Bill> Bills { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public List<int> ItemIds { get; set; } = new List<int>();
        public List<PersonResponse> Members { get; set; }
    }
}
