using API.Helpers;

namespace API.Payloads.Response
{
    public class ItemForAdminResponse : ItemForSystemResponse
    {
        public EItemStatus Status { get; set; }
    }
}
