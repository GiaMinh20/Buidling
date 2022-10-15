namespace API.Payloads.Response
{
    public class ItemUnpaiedResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public int LatedDay { get; set; }
        public int RenterId { get; set; }
        public string RenterName { get; set; }
        public bool HasMonthlyBill { get; set; } = false;
    }
}
