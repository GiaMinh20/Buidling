using API.Helpers;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string AvatarUrl { get; set; }
        public string AvatarId { get; set; }
        public string VideoUrl { get; set; }
        public string VideoId { get; set; }
        public EItemStatus Status { get; set; }
        public string Location { get; set; }

        public bool MonthlyPaied { get; set; } = false;
        public DateTime? MonthlyPaiedDate { get; set; }
        public DateTime? RentedDate { get; set; }
        public virtual TypeItem Type { get; set; }
        public virtual Account Renter { get; set; }

        public ICollection<ItemPhoto> ItemPhotos { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
