namespace WebApiAuthors.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public DateTime RequestDate { get; set; }
        public APIKey Key { get; set; }
    }
}
