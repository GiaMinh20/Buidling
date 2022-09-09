using System;

namespace API.Payloads.Requests
{
    public class CreateNotificationRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        //public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
