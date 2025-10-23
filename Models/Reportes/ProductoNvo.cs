using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Reportes
{
    public class ProductoNvo
    {
        public string SCodigo { get; set; }
        [Required]
        public string? SNombre { get; set; }
        [Required]
        public string SDescripcion { get; set; }
        [Required]
        [JsonRequired] public int ICategoria { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DePrecio { get; set; }
        [JsonRequired] public int IStock { get; set; }
        [Required]
        [JsonRequired] public int IStockMin { get; set; }
        [JsonRequired] public int iProveedor { get; set; }
    }
}
