namespace API.Helpers
{
    public class BillForAdminParam :PaginationParams
    {
        public bool? Paied { get; set; }
        public int? AccountId { get; set; }
        public int? BillId { get; set; }
        public int? ItemId { get; set; }
    }
}
