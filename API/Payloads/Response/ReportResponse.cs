using System;

namespace API.Payloads.Response
{
    public class ReportResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AvatarUrl { get; set; }
        public string Username { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
