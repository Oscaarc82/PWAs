using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Reportes
{
    public class Producto
    {
        [Key]
        public int IId { get; set; }
        public string SCodigo { get; set; }
        [Required]
        public string? SNombre { get; set; }
        [Required]
        public string SDescripcion {  get; set; }
        [Required]
        public int ICategoria { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DePrecio { get; set; }
        public int IStock { get; set; }
        [Required]
        public int IStockMin { get; set; }
    }
}
