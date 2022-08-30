using System;

namespace API.Payloads.Response
{
    public class BillForAdminResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public int SumPrice { get; set; }
        public bool Paied { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
