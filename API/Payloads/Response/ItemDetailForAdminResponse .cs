using API.Entities;
using API.Helpers;
using System.Collections.Generic;

namespace API.Payloads.Response
{
    public class ItemDetailForAdminResponse : ItemDetailForSystemResponse
    {
        public EItemStatus Status { get; set; }
        public string RenterName { get; set; }
        public int NumberOfParent { get; set; }
    }
}
