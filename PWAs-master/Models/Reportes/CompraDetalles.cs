using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Reportes
{
    [Table("tCompraDetalles")]
    public class CompraDetalles
    {
        [Key]
        public int IId { get; set; }
        [Required]
        public int ICompra { get; set; }
        [Required]
        public int IProducto { get; set; }
        [Required]
        public int ICantidad { get; set; }
        [Required]
        [Column("iPrecio", TypeName = "decimal(18, 2)")]
        public decimal? DePrecio { get; set; }
        [ForeignKey("IProducto")]
        public Producto? Producto { get; set; }
    }
}
