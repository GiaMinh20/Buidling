namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }

        public virtual Item Item { get; set; }
        public int ItemId { get; set; }

    }
}
