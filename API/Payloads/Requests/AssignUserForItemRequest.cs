namespace API.Payloads.Requests
{
    public class AssignUserForItemRequest
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
    }
}
