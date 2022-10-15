using System;

namespace API.Payloads.Response
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
