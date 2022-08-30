namespace API.Helpers
{
    public class ItemForSystemParams : PaginationParams
    {
        public string OrderBy { get; set; }
        public string SearchTerm { get; set; }
        public int? Types { get; set; }
        
    }
}
