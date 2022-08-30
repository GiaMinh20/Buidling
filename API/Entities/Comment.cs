using System;

namespace API.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public virtual Account Account { get; set; }
        public int AccountId { get; set; }
        public virtual Item Item { get; set; }
        public int ItemId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
