namespace WebApiAuthors.Entities
{
    public class DomainRestriction
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public string Domain { get; set; }
        public APIKey Key { get; set; }
    }
}
