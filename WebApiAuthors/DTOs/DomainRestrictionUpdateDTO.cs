using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class DomainRestrictionUpdateDTO
    {
        [Required]
        public string Domain { get; set; }
    }
}
