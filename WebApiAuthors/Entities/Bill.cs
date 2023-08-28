namespace WebApiAuthors.Entities
{
    public class Bill
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public AppUser User { get; set; }

        public bool Paid { get; set; }

        public decimal Price { get; set; }

        public DateTime GenerationDate { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
