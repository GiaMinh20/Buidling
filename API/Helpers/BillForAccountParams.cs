namespace API.Helpers
{
    public class BillForAccountParams : PaginationParams
    {
        public int? BillId { get; set; }
        public bool? Paied { get; set; }
    }
}
