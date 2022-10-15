namespace API.Payloads.Requests
{
    public class UnAssignUserForItemRequest
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public bool HasRequest { get; set; } = true;
    }
}
