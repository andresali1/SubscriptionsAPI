using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 80, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [FirstCapitalLetter]
        public string A_Name { get; set; }

        public List<Author_Book> Author_Book { get; set; }
    }
}
