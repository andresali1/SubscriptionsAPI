using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class DomainRestrictionCreateDTO
    {
        public int KeyId { get; set; }

        [Required]
        public string Domain { get; set; }
    }
}
