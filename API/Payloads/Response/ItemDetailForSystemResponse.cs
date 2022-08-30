using API.Entities;
using System.Collections.Generic;

namespace API.Payloads.Response
{
    public class ItemDetailForSystemResponse : ItemForSystemResponse
    {
        public string Description { get; set; }
        public string Location { get; set; }
        public List<string> PictureUrl { get; set; }
        public string VideoUrl { get; set; }
        public List<CommentResponse> Comments { get; set; }
    }
}
