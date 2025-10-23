using System.ComponentModel.DataAnnotations;

namespace PWAs.Models
{
    public class TokenTemp
    {
        [Key]
        public int IIdToken { get; set; }
        public string? SToken { get; set; }
        public int IUsuarioId { get; set; }
        public DateTime? DFechaExpiracion { get; set; }
    }
}
