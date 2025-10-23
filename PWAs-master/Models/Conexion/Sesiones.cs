using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;


namespace PWAs.Models.Conexion
{
    public class Sesiones
    {
        [Key]
        public int IId { get; set; }
        [Required]
        public int IUsuario { get; set; }
        [Required]
        public string SUsuario { get; set; }
        [Required]
        public Point Location { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public DateTime DExpiracion { get; set; }
    }
}
