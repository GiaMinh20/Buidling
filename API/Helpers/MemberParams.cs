namespace API.Helpers
{
    public class MemberParams :PaginationParams
    {
        public string AccountName { get; set; }
        public string Gender { get; set; }
        public string CCCD { get; set; }
        public string FullName { get; set; }
    }
}
