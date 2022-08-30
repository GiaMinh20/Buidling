using System;
using System.Collections.Generic;

namespace API.Payloads.Response
{
    public class ReportDetailResponse : ReportResponse
    {
        public string Content { get; set; }
        public List<string> PictureUrl { get; set; }
    }
}
