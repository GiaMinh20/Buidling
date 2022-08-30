using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class ReportBuilding
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AvatarUrl { get; set; }
        public string AvatarId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public ICollection<ReportPhoto> ReportPhotos { get; set; }
        public virtual Account Account { get; set; }
    }
}
