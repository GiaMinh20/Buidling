using System.Collections.Generic;

namespace API.Entities
{
    public class Favorite
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
