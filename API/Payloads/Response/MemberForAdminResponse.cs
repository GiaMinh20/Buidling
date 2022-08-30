using API.Helpers;
using System;

namespace API.Payloads.Response
{
    public class MemberForAdminResponse : Person
    {
        public string PlaceOfOrigin { get; set; }
        public string MemberOfAccount { get; set; }
        public int MemberOfAccountId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
