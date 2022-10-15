using System.Collections.Generic;

namespace API.Entities
{
    public class Favorite
    {
        public int Id { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
