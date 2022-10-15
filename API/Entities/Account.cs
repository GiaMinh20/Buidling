using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Account : IdentityUser<int>
    {
        public string AvatarUrl { get; set; }
        public string AvatarId { get; set; }
        public List<Item> Items { get; set; }
        public int NumberOfParent { get; set; }
        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        //public virtual Favorite Favorite { get; set; } = new Favorite();
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}