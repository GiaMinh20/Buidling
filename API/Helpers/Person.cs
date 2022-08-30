using API.Entities;
using System;

namespace API.Helpers
{
    public class Person
    {
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string AvatarId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string CCCD { get; set; }
        public string Nationality { get; set; }
    }
}
