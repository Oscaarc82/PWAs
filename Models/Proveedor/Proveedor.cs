using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models
{
    [Table("tProveedor")]
    public class Proveedor
    {
        [Key]
        public int Iid { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(90, ErrorMessage = "El nombre no puede exceder 90 caracteres")]
        public string SNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder 15 caracteres")]
        public string STelefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contacto es obligatorio")]
        [StringLength(60, ErrorMessage = "El contacto no puede exceder 60 caracteres")]
        public string SContacto { get; set; } = string.Empty;
    }
}