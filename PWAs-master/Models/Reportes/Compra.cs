using PWAs.Models.Usuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Reportes
{
    public class Compra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("iId")]
        public int IId { get; set; }
        [Required]
        [Column("iIdUsuario")]
        public int IIdUsuario { get; set; }
        [ForeignKey("IIdUsuario")]
        public Usuario Usuario { get; set; }
        [Required]
        [Column("dFechaCompra")]
        public DateTime DFechaCompra { get; set; }
        [Required]
        [Column("DeTotal", TypeName = "decimal(18, 2)")]
        public decimal? DeTotal { get; set; }
        public ICollection<CompraDetalles> Detalles { get; set; }
    }
}
