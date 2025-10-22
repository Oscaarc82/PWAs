using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models
{
    public class Proveedor
    {
        [Key]
        public int Iid { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "El nombre no puede exceder 90 caracteres")]
        public string SNombre { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder 15 caracteres")]
        public string STelefono { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "El contacto no puede exceder 60 caracteres")]
        public string SContacto { get; set; }
    }
}