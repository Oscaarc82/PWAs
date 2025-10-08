using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Usuarios
{
    public class Usuario
    {
        [Key]
        public int IID { get; set; }
        public string? SNombre { get; set; }

        public string? SEmail { get; set; }

        public string? SPasswd { get; set; }

        public DateTime? DFechaC { get; set; }

        public DateTime? DFechaA { get; set; }

        public int IEstatus { get; set; }

        public int iIdRol { get; set; }

        [ForeignKey("IIdRol")]
        public Rol? Rol { get; set; }
        public string? SOffSalt { get; set; }
    }
}
