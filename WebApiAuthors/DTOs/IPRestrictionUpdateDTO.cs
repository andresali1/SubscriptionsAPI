using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class IPRestrictionUpdateDTO
    {
        [Required]
        public string IP { get; set; }
    }
}
