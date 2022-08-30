namespace API.Payloads.Response
{
    public class ItemForFavoriteResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public string AvatarUrl { get; set; }
        public string Type { get; set; }
    }
}
