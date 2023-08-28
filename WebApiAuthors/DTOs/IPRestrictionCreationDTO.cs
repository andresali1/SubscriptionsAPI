using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class IPRestrictionCreationDTO
    {
        public int KeyId { get; set; }

        [Required]
        public string IP { get; set; }
    }
}
