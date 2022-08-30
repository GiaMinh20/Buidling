using API.Helpers;
using System.Collections.Generic;

namespace API.Payloads.Response
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public List<string> PictureUrl { get; set; }
        public string VideoUrl { get; set; }
        public EItemStatus ItemStatus { get; set; }
        public int TypeId { get; set; }
    }
}
