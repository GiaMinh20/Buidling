namespace API.Helpers
{
    public class Address
    {
        public string City { get; set; }
        public string District { get; set; }
        public string SubDistrict { get; set; }
        public string Street { get; set; }
        public string Details { get; set; }
        public string SumAddress()
        {
            return Details + " " +
                    Street + " " +
                    SubDistrict + " " +
                    District + " " +
                    City;
        }
    }
}
