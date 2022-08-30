using System;

namespace API.Payloads.Response
{
    public class CommentResponse
    {
        public string AccountName { get; set; }
        public string AccountAvatar { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
