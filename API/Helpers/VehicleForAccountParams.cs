namespace API.Helpers
{
    public class VehicleForAccountParams : PaginationParams
    {
        public bool? Status { get; set; }
        public string LicensePlates { get; set; }
        public string Transportation { get; set; }
    }
}
