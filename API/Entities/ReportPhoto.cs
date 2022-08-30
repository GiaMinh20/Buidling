namespace API.Entities
{
    public class ReportPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }

        public virtual ReportBuilding Report { get; set; }
        public int ReportId { get; set; }
    }
}
