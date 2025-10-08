using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Reportes
{
    public class Producto
    {
        [Key]
        public int IId { get; set; }
        [Required]
        public string? SNombre { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DePrecio { get; set; }
        public int IStock { get; set; }
        [Required]
        public int IsDisponible { get; set; }
    }
}
