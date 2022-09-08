namespace API.Payloads.Requests
{
    public class PayRentMoney
    {
        public string CardNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string CVC { get; set; }
        public int BllId { get; set; }
    }
}
