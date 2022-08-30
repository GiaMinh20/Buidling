namespace API.Helpers
{
    public class AccountParams : PaginationParams
    {
        public string AccountName { get; set; }
        public string Email { get; set; }
    }
}
