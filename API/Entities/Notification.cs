using System;

namespace API.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        //public bool Seen { get; set; }
        //public virtual Account Account { get; set; }
    }
}
