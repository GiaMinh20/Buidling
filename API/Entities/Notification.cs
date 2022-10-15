using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
