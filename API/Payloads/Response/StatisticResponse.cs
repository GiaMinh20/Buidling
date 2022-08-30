namespace API.Payloads.Response
{
    public class StatisticResponse
    {
        public int NumberOfPerson { get; set; }
        public int NumberOfVehicle { get; set; }
        public int NumberOfRented { get; set; }
        public int NumberOfReport { get; set; }
        public int NumberOfRentRequestInMonth { get; set; }
        public int NumberOfAccount { get; set; }
        public int NumberOfUnRent { get; set; }
    }
}
