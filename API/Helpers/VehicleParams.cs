namespace API.Helpers
{
    public class VehicleParams : PaginationParams
    {
        public bool? Status { get; set; }
        public string LicensePlates { get; set; }
        public string Transportation { get; set; }
        public string AccountName { get; set; }
    }
}
